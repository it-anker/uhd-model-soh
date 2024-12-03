using Mars.Core.Data;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;

namespace SOH.Process.Server.Simulations.Jobs;

public static class SimulationJobHelper
{
    internal static IEnumerable<Feature> CollectAgentFeatures(IEnumerable<IGeneratedTypeLogger> typeLoggers)
    {
        foreach (var agentLoggers in typeLoggers)
        {
            foreach (var proxy in agentLoggers.EntityProxies.Where(proxy => proxy.IsSerializable()))
            {
                var attributeValues = proxy.SerializeProperties()
                    .ToDictionary(tuple => tuple.Item1, tuple => tuple.Item2);
                yield return new Feature(new Point(proxy.Position.X, proxy.Position.Y),
                    new AttributesTable(attributeValues));
            }
        }
    }
}