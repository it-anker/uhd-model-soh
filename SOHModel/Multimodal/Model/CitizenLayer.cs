using System.Collections.Concurrent;
using Mars.Common.Core;
using Mars.Common.Core.Logging;
using Mars.Core.Data;
using Mars.Interfaces;
using Mars.Interfaces.Data;
using Mars.Interfaces.Layers;
using Mars.Interfaces.Model;
using SOHModel.Car.Parking;
using SOHModel.Multimodal.Layers;
using SOHModel.Multimodal.Multimodal;

namespace SOHModel.Multimodal.Model;

/// <summary>
///     The citizen layer containing all citizen agents.
/// </summary>
public class CitizenLayer : AbstractMultimodalLayer
{
    private static readonly ILogger Logger = LoggerFactory.GetLogger(typeof(CitizenLayer));
    private IDictionary<Guid, Citizen> _agents = new ConcurrentDictionary<Guid, Citizen>();

    /// <summary>
    ///     The data mediator layer to delegate data driven queries
    ///     for agent decision making to different sources.
    /// </summary>
    public MediatorLayer MediatorLayer { get; set; } = default!;

    /// <summary>
    ///     THe parking layer where all vehicles are located when in state parking.
    /// </summary>
    public new CarParkingLayer CarParkingLayer { get; set; } = default!;

    public override bool InitLayer(
        LayerInitData layerInitData,
        RegisterAgent? registerAgentHandle = null,
        UnregisterAgent? unregisterAgent = null)
    {
        base.InitLayer(layerInitData, registerAgentHandle, unregisterAgent);

        AgentMapping? agentInitConfig = layerInitData.AgentInitConfigs.FirstOrDefault();
        if (agentInitConfig?.IndividualMapping == null) return false;

        IAgentManager? agentManager = layerInitData.Container.Resolve<IAgentManager>();
        List<IModelObject> dependencies = new List<IModelObject>
        {
            MediatorLayer, SpatialGraphMediatorLayer, 
            CarParkingLayer, BicycleRentalLayer, FerryStationLayer
        };

        _agents = agentManager.Spawn<Citizen, CitizenLayer>(dependencies)
            .ToDictionary(citizen => citizen.ID, citizen => citizen);

        IDictionary<string, IndividualMapping>? layerParameters = layerInitData.LayerInitConfig.ParameterMapping;
        if (layerParameters.TryGetValue("ParkingOccupancy", out IndividualMapping? mapping))
        {
            double occupiedParkingPercentage = mapping.Value.Value<double>();
            int carCount = _agents.Values.Count(p => p.CapabilityDrivingOwnCar);
            CarParkingLayer?.UpdateOccupancy(occupiedParkingPercentage, carCount);
        }

        Logger.LogInfo("Created Agents: " + _agents.Count);

        return _agents.Count != 0;
    }
}