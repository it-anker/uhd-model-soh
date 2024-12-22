using Mars.Core.Data;
using Mars.Core.Simulation.Entities;
using Mars.Interfaces.Environments;
using Mars.Interfaces.Model;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using SOH.Process.Server.Models.Ogc;
using SOHModel.Domain.Graph;

namespace SOH.Process.Server.Simulations.Jobs;

public static class SimulationResultExtensions
{
    public static void CollectOutputs(this ISerializerManager serializer,
        SimulationWorkflowState step, Execute executionConfig, Result result)
    {
        serializer.CollectAgentResultForFeatureCollection(step, executionConfig, result);
        serializer.CollectAgentRoadOccupationResult(step, executionConfig, result);
        serializer.CollectAgentModalityUsageResult(executionConfig, step, result);
    }

    private static void CollectAgentResultForFeatureCollection(this ISerializerManager serializer,
        SimulationWorkflowState step, Execute executionConfig, Result result)
    {
        const string outputKey = "agents";
        if (!TryGetConfigAndAdd(executionConfig, result, outputKey, out var entry)) return;

        entry.FeatureCollection ??= [];
        var featureCollection = entry.FeatureCollection;
        var typeLoggers = serializer
            .GetTypeLoggers()
            .Where(logger => logger.Mapping is AgentMapping);

        foreach (var typeLogger in typeLoggers)
        {
            long currentTick = step.CurrentTick;
            var serializeAbleProxies =
                typeLogger.EntityProxies.Where(proxy =>
                    (proxy.OutputTicks != null && proxy.OutputTicks.Contains(currentTick)) ||
                    (proxy.OutputFrequency > 0 && currentTick % proxy.OutputFrequency == 0));

            foreach (var entityLogger in serializeAbleProxies.Where(logger => logger.Position != null))
            {
                var point = new Point(entityLogger.Position.X, entityLogger.Position.Y);
                var dictionary = entityLogger.SerializeProperties()
                    .ToDictionary(tuple => tuple.Item1, tuple => tuple.Item2);
                var feature = new Feature(point, new AttributesTable(dictionary));
                featureCollection.Add(feature);
            }
        }
    }

    private static void CollectAgentRoadOccupationResult(
        this ISerializerManager serializer,
        SimulationWorkflowState step, Execute executionConfig, Result result)
    {
        const string outputKey = "soh_output_avg_road_count";
        if (!TryGetConfigAndAdd(executionConfig, result, outputKey, out var entry))
        {
            return;
        }

        entry.TimeSeries ??= [];

        var graphLayer = serializer.LayerLoggers
            .Values.FirstOrDefault(pair =>
                pair.LayerType.MetaType == typeof(SpatialGraphMediatorLayer));

        if (graphLayer?.Entity is ISpatialGraphLayer spatialGraphLayer)
        {
            double averageCount = spatialGraphLayer.Environment.Entities.Count > 0
                ? spatialGraphLayer.Environment.Edges.Values
                    .Average(edge => edge.Entities.Count)
                : 0;
            entry.TimeSeries.Add(new TimeSeriesStep
            {
                DateTime = step.CurrentTimePoint,
                Tick = step.CurrentTick,
                Value = averageCount
            });
        }
    }

    private static void CollectAgentModalityUsageResult(
        this ISerializerManager serializer,
        Execute executionConfig, SimulationWorkflowState step, Result result)
    {
        const string outputKey = "soh_output_sum_modality_usage";
        if (!TryGetConfigAndAdd(executionConfig, result, outputKey, out var entry)) return;

        entry.Value ??= new Dictionary<string, List<TimeSeriesStep>>();

        var groupedResults = (Dictionary<string, List<TimeSeriesStep>>)entry.Value;
        var graphLayer = serializer.LayerLoggers
            .Values.FirstOrDefault(pair =>
                pair.LayerType.MetaType == typeof(SpatialGraphMediatorLayer));

        if (graphLayer?.Entity is not ISpatialGraphLayer spatialGraphLayer) return;

        foreach (var value in Enum.GetValues<SpatialModalityType>())
        {
            List<TimeSeriesStep> series;
            if (groupedResults.TryGetValue(value.ToString(), out var timeSeries))
            {
                series = timeSeries;
            }
            else
            {
                series = [];
                groupedResults.Add(value.ToString(), series);
            }

            int usage = spatialGraphLayer.Environment.Edges.Values
                .Sum(entity => entity.Entities
                    .Count(graphEntity => graphEntity.ModalityType == value));

            series.Add(new TimeSeriesStep
            {
                DateTime = step.CurrentTimePoint,
                Tick = step.CurrentTick,
                Value = usage
            });
        }
    }

    private static bool TryGetConfigAndAdd(Execute executionConfig,
        Result result, string outputKey, out ResultEntry entry)
    {
        if (!executionConfig.Outputs.ContainsKey(outputKey))
        {
            entry = null!;
            return false;
        }

        if (result.Results.TryGetValue(outputKey, out var output))
        {
            entry = output;
            return true;
        }

        entry = new ResultEntry();
        result.Results.Add(outputKey, entry);
        return true;
    }
}