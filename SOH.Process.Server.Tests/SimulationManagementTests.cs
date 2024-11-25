using Mapster;
using Microsoft.Extensions.DependencyInjection;
using SOH.Process.Server.Models.Common.Exceptions;
using SOH.Process.Server.Models.Processes;
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
    public async Task TestCreateSimulation()
    {
        var create = new CreateSimulationProcessRequest
        {
            Title = "SOH Test",
            Description = "my sim desc",
            Keywords = ["planning", "ferry"],
            Version = "1.0.0"
        };

        string simulationId = await _simulationService.CreateAsync(create);
        Assert.NotNull(simulationId);

        var simulation = await _simulationService.GetSimulationAsync(simulationId);
        Assert.Equal("my sim desc", simulation.Description);
        Assert.Equal("SOH Test", simulation.Title);
        Assert.Equal("1.0.0", simulation.Version);
        Assert.Contains("planning", simulation.Keywords);
        Assert.DoesNotContain("soh", simulation.Keywords);
        Assert.Contains("ferry", simulation.Keywords);

        await Assert.ThrowsAsync<NotFoundException>(() =>
            _simulationService.GetSimulationAsync(Guid.NewGuid().ToString()));

        var found = await _simulationService.FindSimulationAsync(simulationId);
        Assert.NotNull(found);
        Assert.Equal(simulationId, found.Id);
        var notFound = await _simulationService.FindSimulationAsync(Guid.NewGuid().ToString());
        Assert.Null(notFound);
    }

    [Fact]
    public async Task TestSearchSimulations()
    {
        for (int i = 0; i < 10; i++)
        {
            var create = new CreateSimulationProcessRequest
            {
                Title = "TestUpdateSimulation" + i,
                Version = "1.0.0",
                Description = "my desc"
            };
            string id = await _simulationService.CreateAsync(create);
            Assert.StartsWith("simulation", id);
        }

        var filter = new ParameterLimit
        {
            PageSize = 1, PageNumber = 1
        };
        var response = await _simulationService.ListProcessesAsync(filter);
        Assert.NotEmpty(response.Processes);
        Assert.Single(response.Processes);
    }

    [Fact]
    public async Task TestUpdateSimulation()
    {
        var create = new CreateSimulationProcessRequest
        {
            Title = "TestUpdateSimulation",
            Description = "my sim desc"
        };
        await Assert.ThrowsAsync<ArgumentNullException>(() => _simulationService.CreateAsync(create));
        create.Version = "1.0.0";
        string simulationId = await _simulationService.CreateAsync(create);
        Assert.NotNull(simulationId);

        var simulation = await _simulationService.GetSimulationAsync(simulationId);
        Assert.Equal("TestUpdateSimulation", simulation.Title);
        var update = simulation.Adapt<UpdateSimulationProcessRequest>();
        Assert.Equal("my sim desc", simulation.Description);
        update.Description = "my updated desc";
        update.JobId = Guid.NewGuid().ToString();
        update.ResultId = Guid.NewGuid().ToString();

        await _simulationService.UpdateAsync(simulationId, update);
        var updatedSimulation = await _simulationService.GetSimulationAsync(simulationId);

        Assert.Equal("my updated desc", updatedSimulation.Description);
        Assert.Equal(update.JobId, updatedSimulation.JobId);
        Assert.Equal(update.ResultId, updatedSimulation.ResultId);
        await Assert.ThrowsAsync<NotFoundException>(() =>
            _simulationService.GetSimulationAsync(Guid.NewGuid().ToString()));
    }

    [Fact]
    public async Task TestDeleteSimulation()
    {
        var create = new CreateSimulationProcessRequest
        {
            Title = "test process",
            Description = "my deleted desc",
            Version = "1.0.0"
        };

        string simulationId = await _simulationService.CreateAsync(create);
        Assert.NotNull(simulationId);

        var found = await _simulationService.FindSimulationAsync(simulationId);
        Assert.NotNull(found);
        await _simulationService.DeleteAsync(simulationId);
        var notFound = await _simulationService.FindSimulationAsync(simulationId);
        Assert.Null(notFound);
        await Assert.ThrowsAsync<NotFoundException>(() =>
            _simulationService.GetSimulationAsync(simulationId));
    }
}