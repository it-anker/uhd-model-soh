using Microsoft.Extensions.DependencyInjection;
using SOH.Process.Server.Simulations;

namespace SOH.Process.Server.Tests;

public class SimulationManagementTests : AbstractManagementTests
{
    private readonly ISimulationService _simulationService;

    public SimulationManagementTests(OgcIntegration services) : base(services)
    {
        _simulationService = Services.GetRequiredService<ISimulationService>();
    }

    [Fact]
    public void TestCreateSimulation()
    {
    }

    [Fact]
    public void TestUpdateSimulation()
    {
    }

    [Fact]
    public void TestDeleteSimulation()
    {
    }
}