using MediatR;
using SOH.Process.Server.Background;
using SOH.Process.Server.Models.Ogc;
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
        return await simulationService.GetSimulationAsync(simulationId, cancellationToken);
    }

    public async Task<SimulationJob> Handle(CreateSimulationJobRequest request, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(request.SimulationId);
        var simulation = await simulationService.GetSimulationAsync(request.SimulationId, cancellationToken);

        string jobId = await simulationService.CreateAsync(new SimulationJob
            { SimulationId = simulation.Id }, cancellationToken);

        var simulationRunRequest = new SimulationRunJobRequest
        {
            JobId = jobId, Execute = request.Execute
        };

        var executionMode = JobControlOptions.SynchronousExecution;
        if (request.Prefer.HasValue)
        {
            executionMode = request.Prefer.Value;
        }
        else if (simulation.JobControlOptions.TrueForAll(options =>
                     options == JobControlOptions.SynchronousExecution))
        {
            executionMode = JobControlOptions.SynchronousExecution;
        }
        else if (simulation.JobControlOptions.TrueForAll(options =>
                     options == JobControlOptions.AsyncExecution))
        {
            executionMode = JobControlOptions.AsyncExecution;
        }

        if (executionMode == JobControlOptions.AsyncExecution)
        {
            string backgroundJobId = jobService.Enqueue(() =>
                mediator.Send(simulationRunRequest, cancellationToken));
            if (!string.IsNullOrEmpty(backgroundJobId))
                await simulationService.UpdateAsync(jobId, backgroundJobId, cancellationToken);
        }
        else if (executionMode == JobControlOptions.SynchronousExecution)
        {
            await mediator.Send(simulationRunRequest, cancellationToken);
        }

        return await simulationService.GetSimulationJobAsync(jobId, cancellationToken);
    }
}