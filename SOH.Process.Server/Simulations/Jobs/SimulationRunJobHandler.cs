using MediatR;

namespace SOH.Process.Server.Simulations.Jobs;

public class SimulationRunJobHandler(ISimulationService simulationService) : IRequestHandler<SimulationRunJobRequest, Unit>
{
    public async Task<Unit> Handle(SimulationRunJobRequest request, CancellationToken cancellationToken)
    {
        var simulation = await simulationService.FindSimulationAsync(request.SimulationId, cancellationToken);
        var job = !string.IsNullOrEmpty(simulation?.JobId)
            ? await simulationService.FindJobAsync(simulation.JobId, cancellationToken) : null;

        if (simulation != null && job != null && simulation.JobId != null)
        {
            job.StartedUtc = DateTime.UtcNow;
            await simulationService.UpdateAsync(job.JobId, job, cancellationToken);

            // TODO: start single simulation
            await Task.Delay(5000, cancellationToken);
        }

        return Unit.Value;
    }
}