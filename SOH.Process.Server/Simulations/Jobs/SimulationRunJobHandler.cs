using MediatR;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using SOH.Process.Server.Models.Ogc;
using SOH.Process.Server.Models.Processes;

namespace SOH.Process.Server.Simulations.Jobs;

public class SimulationRunJobHandler(
    ISimulationService simulationService,
    IResultService resultService)
    : IRequestHandler<SimulationRunJobRequest, Unit>
{
    public async Task<Unit> Handle(SimulationRunJobRequest request, CancellationToken cancellationToken)
    {
        var job = await simulationService.FindJobAsync(request.JobId, cancellationToken);
        var simulation = await simulationService.FindSimulationAsync(
            job?.SimulationId ?? string.Empty, cancellationToken);

        if (simulation != null && job != null)
        {
            job.StartedUtc = DateTime.UtcNow;
            job.Status = StatusCode.Running;
            await simulationService.UpdateAsync(job.JobId, job, cancellationToken);

            // TODO: start single simulation
            for (int i = 1; i <= 10; i++)
            {
                await Task.Delay(100, cancellationToken);
                job.Progress = (i / 10) * 100;
                await simulationService.UpdateAsync(job.JobId, job, cancellationToken);
            }

            job.FinishedUtc = DateTime.UtcNow;
            job.Status = StatusCode.Successful;
            job.ResultId = await resultService.CreateAsync(new Result
            {
                SimulationId = simulation.Id,
                JobId = job.JobId,
                FeatureCollection =
                [
                    new Feature(new Point(0, 0), new AttributesTable
                    {
                        { "field", 1 }
                    })
                ]
            }, cancellationToken);

            await simulationService.UpdateAsync(job.JobId, job, cancellationToken);
        }

        return Unit.Value;
    }
}