using Mars.Components.Services;
using Mars.Interfaces.Data;
using Mars.Interfaces.Layers;
using Mars.Interfaces.Model;
using SOHModel.Multimodal.Multimodal;

namespace SOHModel.Multimodal.Model;

public class DockWorkerLayer : AbstractMultimodalLayer
{
    public DockWorkerLayer()
    {
        Agents = new Dictionary<Guid, DockWorker>();
    }

    public IDictionary<Guid, DockWorker> Agents { get; set; }

    public override bool InitLayer(
        LayerInitData layerInitData,
        RegisterAgent registerAgentHandle = null,
        UnregisterAgent unregisterAgent = null)
    {
        base.InitLayer(layerInitData, registerAgentHandle, unregisterAgent);

        AgentMapping? agentMapping =
            layerInitData.AgentInitConfigs.FirstOrDefault(mapping =>
                mapping.ModelType.MetaType == typeof(DockWorker));

        if (agentMapping != null)
            Agents = AgentManager.SpawnAgents<DockWorker>(agentMapping, registerAgentHandle, unregisterAgent,
                new List<ILayer> { this });

        return true;
    }
}