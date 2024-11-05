namespace SOH.Process.Server.Simulations;

public interface ISimulationService
{
    Task<Guid> CreateAsync(CreateSimulationRequest request, CancellationToken token = default);

    Task UpdateAsync(UpdateSimulationRequest request, CancellationToken token = default);

    Task DeleteAsync(Guid simulationId, CancellationToken token = default);

    Task<Simulation> GetAsync(Guid simulationId, CancellationToken token = default);
}