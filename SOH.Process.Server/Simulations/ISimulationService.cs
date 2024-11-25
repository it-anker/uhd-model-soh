using SOH.Process.Server.Models.Ogc;
using SOH.Process.Server.Models.Processes;

namespace SOH.Process.Server.Simulations;

public interface ISimulationService
{
    Task<string> CreateAsync(CreateSimulationProcessRequest request, CancellationToken token = default);

    Task<string> CreateAsync(SimulationJob request, CancellationToken token = default);

    Task UpdateAsync(string simulationId, UpdateSimulationProcessRequest request, CancellationToken token = default);

    Task UpdateAsync(string jobId, SimulationJob request, CancellationToken token = default);

    Task DeleteAsync(string id, CancellationToken token = default);

    Task<SimulationProcess> GetSimulationAsync(string simulationId, CancellationToken token = default);

    Task<SimulationJob> GetSimulationJobAsync(string jobId, CancellationToken token = default);

    Task<SimulationProcess?> FindSimulationAsync(string simulationId, CancellationToken token = default);

    Task<SimulationJob?> FindJobAsync(string jobId, CancellationToken token = default);

    Task<JobList> ListJobsAsync(CancellationToken token = default);

    Task<ProcessList> ListProcessesAsync(ParameterLimit simulation, CancellationToken token = default);

    Task<ParameterLimitResponse<SimulationProcess>> ListProcessesPaginatedAsync(
        ParameterLimit simulation, CancellationToken token = default);
}