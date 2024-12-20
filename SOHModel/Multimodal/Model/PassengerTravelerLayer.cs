using Mars.Components.Services;
using Mars.Interfaces.Data;
using Mars.Interfaces.Layers;
using SOHModel.Multimodal.Multimodal;

namespace SOHModel.Multimodal.Model;

/// <summary>
///     A <see cref="IMultimodalLayer"/> that spawns <see cref="PassengerTraveler"/> agents.
/// </summary>
public class PassengerTravelerLayer : AbstractMultimodalLayer
{
    public IDictionary<Guid, PassengerTraveler> Agents { get; set; } = new Dictionary<Guid, PassengerTraveler>();

    public override bool InitLayer(
        LayerInitData layerInitData,
        RegisterAgent? registerAgentHandle = null,
        UnregisterAgent? unregisterAgent = null)
    {
        bool initiated = base.InitLayer(layerInitData, registerAgentHandle, unregisterAgent);

        var agentMapping =
            layerInitData.AgentInitConfigs.FirstOrDefault(mapping =>
                mapping.ModelType.MetaType == typeof(PassengerTraveler));

        if (agentMapping != null && registerAgentHandle != null && unregisterAgent != null)
        {
            Agents = AgentManager.SpawnAgents<PassengerTraveler>(agentMapping,
                registerAgentHandle, unregisterAgent, [this]);
        }

        return initiated;
    }
}