using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SOH.Process.Server.Models.Ogc;
using SOH.Process.Server.Models.Processes;
using SOH.Process.Server.Simulations;
using SOH.Process.Server.Tests.Base;

namespace SOH.Process.Server.Tests;

public class SimulationFullTests : AbstractFullManagementTests
{
    private readonly ISimulationService _simulationService;
    private readonly IResultService _resultService;

    public SimulationFullTests(OgcFullIntegration services) : base(services)
    {
        _simulationService = Services.GetRequiredService<ISimulationService>();
        _resultService = Services.GetRequiredService<IResultService>();
    }

    [Fact]
    public async Task TestRunSimulationAsync()
    {
        var create = new CreateSimulationProcessRequest
        {
            Title = "TestRunSimulationAsync",
            Version = "1.0.0",
            Description = "my async sim desc",
            JobControlOptions = [JobControlOptions.AsyncExecution]
        };

        string simulationId = await _simulationService.CreateAsync(create);
        Assert.NotNull(simulationId);

        var simulation = await _simulationService.GetSimulationAsync(simulationId);
        Assert.Equal("TestRunSimulationAsync", simulation.Title);

        var createJobHandler = Services.GetRequiredService
            <IRequestHandler<CreateSimulationJobRequest, SimulationJob>>();

        var job = await createJobHandler.Handle(new CreateSimulationJobRequest
        {
            SimulationId = simulationId, Execute = new Execute()
        }, CancellationToken.None);

        bool finished = false;
        while (!finished)
        {
            await Task.Delay(500);
            var loaded = await _simulationService.GetSimulationJobAsync(job.JobId);
            finished = loaded.Status == StatusCode.Successful;
        }

        var loadedJob = await _simulationService.GetSimulationJobAsync(job.JobId);
        Assert.NotNull(loadedJob.HangfireJobKey);
        Assert.NotNull(loadedJob.ResultId);
        var result = await _resultService.GetAsync(loadedJob.ResultId);
        Assert.NotNull(result.FeatureCollection);
    }
}