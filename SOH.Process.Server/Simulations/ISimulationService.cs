using SOH.Process.Server.Models.Ogc;
using SOH.Process.Server.Models.Processes;

namespace SOH.Process.Server.Simulations;

public interface ISimulationService
{
    Task<string> CreateAsync(CreateSimulationProcessDescriptionRequest request, CancellationToken token = default);

    Task<string> CreateAsync(string id, CreateSimulationProcessDescriptionRequest descriptionRequest, CancellationToken token = default);

    Task<string> CreateAsync(SimulationJob request, CancellationToken token = default);

    Task UpdateAsync(string simulationId, UpdateSimulationProcessDescriptionRequest descriptionRequest, CancellationToken token = default);

    Task UpdateAsync(string jobId, SimulationJob request, CancellationToken token = default);

    Task UpdateAsync(string jobId, string backgroundJobId, CancellationToken token = default);

    Task<SimulationJob> CancelJobAsync(string jobId, CancellationToken token = default);

    Task DeleteAsync(string id, CancellationToken token = default);

    Task<SimulationProcessDescription> GetSimulationAsync(string simulationId, CancellationToken token = default);

    Task<SimulationJob> GetSimulationJobAsync(string jobId, CancellationToken token = default);

    Task<SimulationProcessDescription?> FindSimulationAsync(string simulationId, CancellationToken token = default);

    Task<SimulationJob?> FindJobAsync(string jobId, CancellationToken token = default);

    Task<JobList> ListJobsAsync(CancellationToken token = default);

    Task<JobList> ListJobsAsync(SearchJobProcessRequest request, CancellationToken token = default);

    Task<ParameterLimitResponse<SimulationJob>> ListJobsPaginatedAsync(
        SearchJobProcessRequest request, CancellationToken token = default);

    Task<ProcessList> ListProcessesAsync(SearchProcessRequest request, CancellationToken token = default);

    Task<ParameterLimitResponse<SimulationProcessDescription>> ListProcessesPaginatedAsync(
        SearchProcessRequest request, CancellationToken token = default);
}