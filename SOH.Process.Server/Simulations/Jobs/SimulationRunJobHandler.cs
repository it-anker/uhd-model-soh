using System.Globalization;
using Mars.Common.Core;
using Mars.Components.Starter;
using Mars.Core.Data;
using Mars.Core.Simulation;
using Mars.Core.Simulation.Entities;
using Mars.Interfaces.Model;
using Mars.Interfaces.Model.Options;
using MediatR;
using Microsoft.Extensions.Localization;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using SOH.Process.Server.Models.Common.Exceptions;
using SOH.Process.Server.Models.Ogc;
using SOH.Process.Server.Models.Processes;
using SOH.Process.Server.Resources;
using SOH.Process.Server.Simulations.Workflows;
using SOHModel.Domain.Graph;

namespace SOH.Process.Server.Simulations.Jobs;

public class SimulationRunJobHandler(
    ISimulationService simulationService,
    IResultService resultService,
    JobSubscribeWorkflow jobSubscribeWorkflow,
    IStringLocalizer<SharedResource> localization)
    : IRequestHandler<SimulationRunJobRequest, Unit>
{
    public async Task<Unit> Handle(SimulationRunJobRequest request, CancellationToken cancellationToken)
    {
        var job = await simulationService.FindJobAsync(request.JobId, cancellationToken);
        var process = await simulationService.FindSimulationAsync(
            job?.ProcessId ?? string.Empty, cancellationToken);

        if (process == null || job == null) return Unit.Value;

        job.StartedUtc = DateTime.UtcNow;
        job.Status = StatusCode.Running;
        await simulationService.UpdateAsync(job.JobId, job, cancellationToken);

        if (request.IsTest)
        {
            var testRunner = new SimulationTestRunHandler(simulationService, resultService, localization);
            await testRunner.ExecuteTestProcess(request, request.JobId, process, cancellationToken);
        }
        else if (GlobalConstants.AvailableModelIds.Contains(process.Id))
        {
            var (description, simConfig) =
                await ConstructModelAndConfig(job.ExecutionConfig, process, job, cancellationToken);
            await ExecuteSimulationProcess(description, simConfig, request.JobId, process, cancellationToken);
        }

        var currentJob = await simulationService.GetSimulationJobAsync(request.JobId, cancellationToken);
        currentJob.FinishedUtc = DateTime.UtcNow;
        await simulationService.UpdateAsync(currentJob.JobId, currentJob, cancellationToken);

        await jobSubscribeWorkflow.NotifySubscriberAsync(currentJob.JobId, cancellationToken);
        return Unit.Value;
    }

    private async Task ExecuteSimulationProcess(ModelDescription description, SimulationConfig simConfig,
        string jobId, SimulationProcessDescription processDescription, CancellationToken token)
    {
        var currentJob = await simulationService.GetSimulationJobAsync(jobId, token);
        try
        {
            var application = SimulationStarter.BuildApplication(description, simConfig);
            var simulation = application.Resolve<ISimulation>();
            var step = simulation.PrepareSimulation(description, simConfig);

            var serializer = application.Resolve<ISerializerManager>();

            var featureCollection = currentJob.ExecutionConfig.Outputs.ContainsKey("agents")
                ? new FeatureCollection()
                : null;
            var timeSeries = currentJob.ExecutionConfig.Outputs.ContainsKey("avg_road_count")
                ? new List<TimeSeriesStep>()
                : null;

            CollectAgentResultForFeatureCollection(serializer, step, featureCollection);
            CollectAgentRoadOccupationResult(serializer, step, timeSeries);
            while (!step.IsFinished)
            {
                if (currentJob.IsCancellationRequested)
                {
                    break;
                }

                if (!double.IsNaN(step.ProgressInPercentage))
                {
                    currentJob.Progress = Math.Min(Convert.ToInt32(step.ProgressInPercentage), 100);
                    currentJob.Message = string.Format(CultureInfo.InvariantCulture,
                        localization["simulation_progress {0}"], step.CurrentTimePoint);
                    await simulationService.UpdateAsync(currentJob.JobId, currentJob, token);
                    await jobSubscribeWorkflow.NotifySubscribersForJobAsync(currentJob, token);
                }

                step = simulation.StepSimulation();
                currentJob = await simulationService.GetSimulationJobAsync(jobId, token);
                CollectAgentResultForFeatureCollection(serializer, step, featureCollection);
                CollectAgentRoadOccupationResult(serializer, step, timeSeries);
            }

            var result = new Result
            {
                ProcessId = processDescription.Id, JobId = currentJob.JobId
            };

            if (featureCollection != null)
            {
                result.Results.Add("agents", new ResultEntry { FeatureCollection = featureCollection });
            }

            if (timeSeries != null)
            {
                result.Results.Add("avg_road_count", new ResultEntry { TimeSeries = timeSeries });
            }

            currentJob.ResultId = await resultService.CreateAsync(result, token);
            currentJob.Status = currentJob.IsCancellationRequested ? StatusCode.Dismissed : StatusCode.Successful;

            if (currentJob.Status == StatusCode.Successful)
            {
                currentJob.Message = string.Format(CultureInfo.InvariantCulture,
                    localization["simulation_finised {0}"], step.CurrentTimePoint);
            }
        }
        catch (Exception exception)
        {
            currentJob.ExceptionMessage = exception.Message;
            currentJob.Status = StatusCode.Failed;
        }

        await simulationService.UpdateAsync(currentJob.JobId, currentJob, token);
        if (Directory.Exists($"results_{simConfig.SimulationIdentifier}"))
        {
            Directory.Delete($"results_{simConfig.SimulationIdentifier}", true);
        }
    }

    private static void CollectAgentResultForFeatureCollection(ISerializerManager serializer,
        SimulationWorkflowState step, FeatureCollection? featureCollection)
    {
        if (featureCollection == null) return;

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

                if (featureCollection.BoundingBox == null)
                {
                    featureCollection.BoundingBox = feature.BoundingBox;
                }
                else
                {
                    featureCollection.BoundingBox.ExpandToInclude(point.X, point.Y);
                }
            }
        }
    }

    private static void CollectAgentRoadOccupationResult(
        ISerializerManager serializer,
        SimulationWorkflowState step,
        List<TimeSeriesStep>? timeSeries)
    {
        if (timeSeries == null) return;

        var graphLayer = serializer.LayerLoggers
            .Values.FirstOrDefault(pair =>
                pair.LayerType.MetaType == typeof(SpatialGraphMediatorLayer));

        if (graphLayer?.Entity is ISpatialGraphLayer spatialGraphLayer)
        {
            double averageCount = spatialGraphLayer.Environment.Entities.Count > 0
                ? spatialGraphLayer.Environment.Edges.Values
                    .Average(edge => edge.Entities.Count)
                : 0;
            timeSeries.Add(new TimeSeriesStep
            {
                DateTime = step.CurrentTimePoint,
                Tick = step.CurrentTick,
                Value = averageCount
            });
        }
    }

    private async Task<(ModelDescription description, SimulationConfig simConfig)> ConstructModelAndConfig(
        Execute executionConfig, SimulationProcessDescription processDescription,
        SimulationJob job, CancellationToken cancellationToken)
    {
        var description = SOHModel.Startup.CreateModelDescription();
        var simConfig = await GetSimulationConfigOrDefault(processDescription, job, executionConfig, cancellationToken);

        UpdateConfigFromInputs(simConfig, executionConfig);
        return (description, simConfig);
    }

    private void UpdateConfigFromInputs(SimulationConfig simConfig, Execute executionConfig)
    {
        if (executionConfig.Inputs.TryGetValue("startPoint", out object? start))
        {
            simConfig.Globals.StartPoint = start.Value<DateTime>();
        }

        if (executionConfig.Inputs.TryGetValue("endPoint", out object? end))
        {
            simConfig.Globals.EndPoint = end.Value<DateTime>();
        }

        if (executionConfig.Inputs.TryGetValue("steps", out object? steps))
        {
            simConfig.Globals.EndPoint = simConfig.Globals.StartPoint
                .GetValueOrDefault().AddSeconds(steps.Value<int>());
        }

        // Ensure not exceed the default
        var range = simConfig.Globals.EndPoint.GetValueOrDefault() - simConfig.Globals.StartPoint.GetValueOrDefault();
        if (range.Seconds > GlobalConstants.DefaultStepRange)
        {
            simConfig.Globals.EndPoint = simConfig.Globals.StartPoint
                .GetValueOrDefault()
                .AddSeconds(GlobalConstants.DefaultStepRange);
        }
    }

    private async Task<SimulationConfig> GetSimulationConfigOrDefault(
        SimulationProcessDescription processDescription, SimulationJob job,
        Execute? processConfig, CancellationToken token)
    {
        SimulationConfig config;
        if (processConfig?.Inputs.TryGetValue("config", out object? configInput) == true &&
            configInput is string configJson)
        {
            try
            {
                config = SimulationConfig.Deserialize(configJson);
            }
            catch (Exception ex)
            {
                await simulationService.DeleteAsync(job.JobId, token);
                throw new BadRequestException(string.Format(CultureInfo.InvariantCulture,
                    localization["Invalid 'config' input for selected process '{0}'"],
                    processDescription.Title), [ex.Message]);
            }
        }
        else if (processDescription.Id == GlobalConstants.FerryTransferId)
        {
            string file = await File.ReadAllTextAsync(GlobalConstants.FerryTransferDefaultConfig, token);
            config = SimulationConfig.Deserialize(file);
        }
        else if (processDescription.Id == GlobalConstants.Green4BikesId)
        {
            string file = await File.ReadAllTextAsync(GlobalConstants.Green4BikesDefaultConfig, token);
            config = SimulationConfig.Deserialize(file);
        }
        else
        {
            throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture,
                localization["Missing simulation config for smart open hamburg process '{0}'"],
                processDescription.Title));
        }

        config.SimulationIdentifier = Guid.NewGuid().ToString();
        config.Globals.ShowConsoleProgress = false;
        config.Globals.OutputTarget = OutputTargetType.None;
        config.Globals.LiveVisualization = false;

        config.Globals.GeoJsonOptions ??= new GeoJsonOptions();
        config.Globals.GeoJsonOptions.OutputPath = $"results_{config.SimulationIdentifier}";

        foreach (var mapping in config.TypeMappings)
        {
            mapping.LiveVisualization = null;

            if (mapping is AgentMapping)
            {
                mapping.OutputTarget = OutputTargetType.Manual;
            }
            else
            {
                mapping.OutputTarget = OutputTargetType.None;
            }
        }

        return config;
    }
}