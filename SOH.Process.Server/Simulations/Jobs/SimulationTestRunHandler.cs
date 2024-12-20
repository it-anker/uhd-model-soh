using Microsoft.Extensions.Localization;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using SOH.Process.Server.Models.Ogc;
using SOH.Process.Server.Models.Processes;
using SOH.Process.Server.Resources;

namespace SOH.Process.Server.Simulations.Jobs;

internal class SimulationTestRunHandler(
    ISimulationService simulationService,
    IResultService resultService,
    IStringLocalizer<SharedResource> localization)
{
    internal async Task ExecuteTestProcess(
        SimulationRunJobRequest request,
        string jobId, SimulationProcessDescription processDescription,
        CancellationToken cancellationToken)
    {
        var currentJob = await simulationService.GetSimulationJobAsync(jobId, cancellationToken);
        try
        {
            for (int i = 1; i <= 10; i++)
            {
                if (currentJob.ExecutionConfig != null && currentJob.ExecutionConfig.Inputs.ContainsKey("errorInSim"))
                {
                    throw new InvalidOperationException("error during sim");
                }
                await Task.Delay(100, cancellationToken);
                currentJob.Progress = (i / 10) * 100;
                await simulationService.UpdateAsync(currentJob.JobId, currentJob, cancellationToken);
            }

            currentJob.Message = localization["process_finished"];
            currentJob.Status = StatusCode.Successful;
            FeatureCollection featureCollection =
            [
                new Feature(new Point(0, 0), new AttributesTable
                {
                    { "field", 1 }
                })
            ];
            currentJob.ResultId = await resultService.CreateAsync(new Result
            {
                ProcessId = processDescription.Id,
                JobId = currentJob.JobId,
                Results = new Dictionary<string, ResultEntry>
                {
                    {
                        "default", new ResultEntry{FeatureCollection = featureCollection}
                    }
                }
            }, cancellationToken);
        }
        catch (Exception exception)
        {
            currentJob.ExceptionMessage = exception.Message;
            currentJob.Status = StatusCode.Failed;
        }
        await simulationService.UpdateAsync(currentJob.JobId, currentJob, cancellationToken);
    }
}