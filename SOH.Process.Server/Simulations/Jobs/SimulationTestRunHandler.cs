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
    internal async Task ExecuteTestProcess(SimulationRunJobRequest request,
        string jobId, SimulationProcessDescription processDescription, CancellationToken cancellationToken)
    {
        var currentJob = await simulationService.GetSimulationJobAsync(jobId, cancellationToken);
        try
        {
            for (int i = 1; i <= 10; i++)
            {
                if (request.Execute != null && request.Execute.Inputs.TryGetValue("func",
                        out object? function) && function is Action<int, SimulationJob> action)
                {
                    action.Invoke(i, currentJob);
                }
                await Task.Delay(100, cancellationToken);
                currentJob.Progress = (i / 10) * 100;
                await simulationService.UpdateAsync(currentJob.JobId, currentJob, cancellationToken);
            }

            currentJob.Message = localization["process_finished"];
            currentJob.Status = StatusCode.Successful;
            currentJob.ResultId = await resultService.CreateAsync(new Result
            {
                ProcessId = processDescription.Id,
                JobId = currentJob.JobId,
                FeatureCollection =
                [
                    new Feature(new Point(0, 0), new AttributesTable
                    {
                        { "field", 1 }
                    })
                ]
            }, cancellationToken);
        }
        catch
        {
            currentJob.Status = StatusCode.Failed;
        }
        await simulationService.UpdateAsync(currentJob.JobId, currentJob, cancellationToken);
    }
}