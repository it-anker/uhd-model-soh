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
    public async Task<string> CreateAsync(
        CreateSimulationProcessDescriptionRequest descriptionRequest,
        CancellationToken token = default)
    {
        return await CreateAsync($"simulation:{Guid.NewGuid()}", descriptionRequest, token);
    }

    public async Task<string> CreateAsync(string id, CreateSimulationProcessDescriptionRequest descriptionRequest, CancellationToken token = default)
    {
        var simulation = new SimulationProcessDescription
        {
            Id = id
        };

        simulation.Update(descriptionRequest);
        await simulationRepository.UpsertAsync(simulation.Id, simulation, token);
        return simulation.Id;
    }

    public async Task<string> CreateAsync(SimulationJob request, CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(request.ProcessId);

        var simulationJob = request.Adapt<SimulationJob>();
        simulationJob.JobId = $"job:{request.ProcessId}:{request}:{Guid.NewGuid()}";
        simulationJob.ProcessId = request.ProcessId;
        request.Message ??= localizer["simulation created"];
        simulationJob.Update(request);

        await simulationRepository.UpsertAsync(simulationJob.JobId, simulationJob, token);
        return simulationJob.JobId;
    }

    public async Task UpdateAsync(string simulationId,
        UpdateSimulationProcessDescriptionRequest descriptionRequest, CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(simulationId);
        var simulation = await GetSimulationAsync(simulationId, token);
        simulation.Update(descriptionRequest);
        simulation.UpdatedUtc = DateTime.UtcNow;
        await simulationRepository.UpsertAsync(simulationId, simulation, token);
    }

    public async Task UpdateAsync(string jobId, SimulationJob request, CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(request.ProcessId);
        var simulationJob = await GetSimulationJobAsync(jobId, token);
        simulationJob.Update(request);
        await simulationRepository.UpsertAsync(jobId, simulationJob, token);
    }

    public async Task UpdateAsync(string jobId, string backgroundJobId, CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(backgroundJobId);
        var simulationJob = await GetSimulationJobAsync(jobId, token);
        simulationJob.HangfireJobKey = backgroundJobId;
        await simulationRepository.UpsertAsync(jobId, simulationJob, token);
    }

    public async Task<SimulationJob> CancelJobAsync(string jobId, CancellationToken token = default)
    {
        var simulationJob = await GetSimulationJobAsync(jobId, token);
        if (simulationJob.Status == StatusCode.Running)
        {
            simulationJob.IsCancellationRequested = true;
            simulationJob.Status = StatusCode.Dismissed;
        }
        await simulationRepository.UpsertAsync(jobId, simulationJob, token);
        return simulationJob;
    }

    public async Task DeleteAsync(string id, CancellationToken token = default)
    {
        await simulationRepository.DeleteAsync<SimulationProcessDescription>(id, token);
        await simulationRepository.DeleteAsync<SimulationJob>(id, token);
    }

    public async Task<SimulationProcessDescription> GetSimulationAsync(string simulationId, CancellationToken token = default)
    {
        return await simulationRepository.FindAsync<SimulationProcessDescription>(simulationId, token) ??
               throw new NotFoundException(localizer[$"simulation with id {simulationId} could not be found"]);
    }

    public async Task<SimulationJob> GetSimulationJobAsync(string jobId, CancellationToken token = default)
    {
        return await simulationRepository.FindAsync<SimulationJob>(jobId, token) ??
               throw new NotFoundException(localizer[$"job with id {jobId} could not be found"]);
    }

    public async Task<SimulationProcessDescription?> FindSimulationAsync(string simulationId, CancellationToken token = default)
    {
        return await simulationRepository.FindAsync<SimulationProcessDescription>(simulationId, token);
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
            .ListPaginatedAsync<SimulationProcessDescription>("simulation*", simulation, token);

        return new ProcessList
        {
            Processes = processes.Data.OfType<ProcessSummary>().ToList()
        };
    }

    public Task<ParameterLimitResponse<SimulationProcessDescription>> ListProcessesPaginatedAsync(ParameterLimit simulation, CancellationToken token = default)
    {
        return simulationRepository
            .ListPaginatedAsync<SimulationProcessDescription>("simulation", simulation, token);
    }

    public Task<ParameterLimitResponse<SimulationProcessDescription>> ListProcessesPaginatedAsync(
        string query, ParameterLimit simulation, CancellationToken token = default)
    {
        return simulationRepository.ListPaginatedAsync<SimulationProcessDescription>(query, simulation, token);
    }
}