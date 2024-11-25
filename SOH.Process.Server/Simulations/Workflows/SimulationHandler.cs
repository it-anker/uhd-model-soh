using Mapster;
using MediatR;
using SOH.Process.Server.Background;
using SOH.Process.Server.Simulations.Jobs;

namespace SOH.Process.Server.Simulations.Workflows;

public class SimulationHandler(
    ISimulationService simulationService,
    IJobService jobService,
    IMediator mediator)
    :
        IRequestHandler<CreateSimulationProcessRequest, SimulationProcess>,
        IRequestHandler<CreateSimulationJobRequest, SimulationJob>
{
    public async Task<SimulationProcess> Handle(CreateSimulationProcessRequest processRequest, CancellationToken cancellationToken)
    {
        string simulationId = await simulationService.CreateAsync(processRequest, cancellationToken);
        var simulation = await simulationService.GetSimulationAsync(simulationId, cancellationToken);
        var simulationRunRequest = new SimulationRunJobRequest { SimulationId = simulationId };

        string jobId = await simulationService.CreateAsync(new SimulationJob
            { SimulationId = simulationId }, cancellationToken);

        var update = simulation.Adapt<UpdateSimulationProcessRequest>();
        update.JobId = jobId;
        await simulationService.UpdateAsync(simulationId, update, cancellationToken);

        var job = await simulationService.GetSimulationJobAsync(jobId, cancellationToken);
        await simulationService.UpdateAsync(jobId, job, cancellationToken);
        string backgroundJobId = jobService.Enqueue(() =>
            mediator.Send(simulationRunRequest, cancellationToken));
        job.JobStatusKey = backgroundJobId;

        return await simulationService.GetSimulationAsync(simulationId, cancellationToken);
    }

    public Task<SimulationJob> Handle(CreateSimulationJobRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}