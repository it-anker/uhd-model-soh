namespace SOH.Process.Server.Simulations.Services;

public class SimulationServiceImpl : ISimulationService
{
    public Task<Guid> CreateAsync(CreateSimulationRequest request, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(UpdateSimulationRequest request, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(Guid simulationId, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public Task<Simulation> GetAsync(Guid simulationId, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }
}