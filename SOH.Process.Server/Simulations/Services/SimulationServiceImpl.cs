using Mapster;
using Microsoft.Extensions.Localization;
using SOH.Process.Server.Models.Common.Exceptions;
using SOH.Process.Server.Models.Ogc;
using SOH.Process.Server.Models.Processes;
using SOH.Process.Server.Persistence;
using SOH.Process.Server.Resources;

namespace SOH.Process.Server.Simulations.Services;

public class SimulationServiceImpl(
    IPersistence simulationRepository,
    IStringLocalizer<SharedResource> localizer) : ISimulationService
{
    public async Task<string> CreateAsync(CreateSimulationProcessRequest request, CancellationToken token = default)
    {
        var simulation = new SimulationProcess
        {
            Id = $"simulation:{Guid.NewGuid()}"
        };

        simulation.Update(request);
        await simulationRepository.UpsertAsync(simulation.Id, simulation, token);
        return simulation.Id;
    }

    public async Task<string> CreateAsync(SimulationJob request, CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(request.SimulationId);

        var simulationJob = request.Adapt<SimulationJob>();

        simulationJob.JobId = $"job:{Guid.NewGuid()}";
        simulationJob.SimulationId = request.SimulationId;
        request.Message ??= localizer["simulation created"];
        simulationJob.Update(request);

        await simulationRepository.UpsertAsync(simulationJob.JobId, simulationJob, token);
        return simulationJob.JobId;
    }

    public async Task UpdateAsync(string simulationId,
        UpdateSimulationProcessRequest request, CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(simulationId);
        var simulation = await GetSimulationAsync(simulationId, token);
        simulation.Update(request);
        simulation.UpdatedUtc = DateTime.UtcNow;
        await simulationRepository.UpsertAsync(simulationId, simulation, token);
    }

    public async Task UpdateAsync(string jobId, SimulationJob request, CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(request.SimulationId);
        var simulationJob = await GetSimulationJobAsync(jobId, token);
        simulationJob.Update(request);
        await simulationRepository.UpsertAsync(jobId, simulationJob, token);
    }

    public async Task DeleteSimulationAsync(string id, CancellationToken token = default)
    {
        await simulationRepository.DeleteAsync<SimulationProcess>(id, token);
        await simulationRepository.DeleteAsync<SimulationJob>(id, token);
    }

    public async Task<SimulationProcess> GetSimulationAsync(string simulationId, CancellationToken token = default)
    {
        return await simulationRepository.FindAsync<SimulationProcess>(simulationId, token) ??
               throw new NotFoundException(localizer[$"simulation with id {simulationId} could not be found"]);
    }

    public async Task<SimulationJob> GetSimulationJobAsync(string jobId, CancellationToken token = default)
    {
        return await simulationRepository.FindAsync<SimulationJob>(jobId, token) ??
               throw new NotFoundException(localizer[$"job with id {jobId} could not be found"]);
    }

    public async Task<SimulationProcess?> FindSimulationAsync(string simulationId, CancellationToken token = default)
    {
        return await simulationRepository.FindAsync<SimulationProcess>(simulationId, token);
    }

    public async Task<SimulationJob?> FindJobAsync(string jobId, CancellationToken token = default)
    {
        return await simulationRepository.FindAsync<SimulationJob>(jobId, token);
    }

    public Task<JobList> ListJobsAsync(CancellationToken token = default)
    {
        var jobs = simulationRepository
            .ListAsync<SimulationJob>("job", token)
            .ToBlockingEnumerable(token);

        return Task.FromResult(new JobList
        {
            Jobs = jobs.OfType<StatusInfo>().ToList()
        });
    }

    public async Task<ProcessList> ListProcessesAsync(ParameterLimit simulation, CancellationToken token = default)
    {
        var processes = await simulationRepository
            .ListPaginatedAsync<SimulationProcess>("simulation*", simulation, token);

        return new ProcessList
        {
            Processes = processes.Data.OfType<ProcessSummary>().ToList()
        };
    }

    public Task<ParameterLimitResponse<SimulationProcess>> ListProcessesPaginatedAsync(ParameterLimit simulation, CancellationToken token = default)
    {
        return simulationRepository
            .ListPaginatedAsync<SimulationProcess>("simulation", simulation, token);
    }
}