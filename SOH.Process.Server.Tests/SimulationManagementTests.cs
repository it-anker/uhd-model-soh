using System.Linq.Expressions;
using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using Mapster;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using SOH.Process.Server.Models.Common.Exceptions;
using SOH.Process.Server.Models.Ogc;
using SOH.Process.Server.Models.Processes;
using SOH.Process.Server.Simulations;
using SOH.Process.Server.Simulations.Jobs;
using SOH.Process.Server.Tests.Base;

namespace SOH.Process.Server.Tests;

public class SimulationManagementTests : AbstractManagementTests
{
    private readonly ISimulationService _simulationService;
    private readonly IResultService _resultService;

    public SimulationManagementTests(OgcIntegration services) : base(services)
    {
        _simulationService = Services.GetRequiredService<ISimulationService>();
        _resultService = Services.GetRequiredService<IResultService>();
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
        update.Version = "1.0.1";

        await _simulationService.UpdateAsync(simulationId, update);
        var updatedSimulation = await _simulationService.GetSimulationAsync(simulationId);

        Assert.Equal("my updated desc", updatedSimulation.Description);
        Assert.Equal("1.0.1", updatedSimulation.Version);
        await Assert.ThrowsAsync<NotFoundException>(() =>
            _simulationService.GetSimulationAsync(Guid.NewGuid().ToString()));
    }

    [Fact]
    public async Task TestDefaultSimulation()
    {
        var existingProcess = await _simulationService.ListProcessesAsync(new ParameterLimit
        {
            PageSize = 1000
        });
        Assert.Contains(existingProcess.Processes, summary => summary.Title?.Contains("Ferry Transfer") == true);
    }

    [Fact]
    public async Task TestRunSimulationSync()
    {
        var create = new CreateSimulationProcessRequest
        {
            Title = "TestRunSimulation",
            Version = "1.0.0",
            Description = "my sim desc",
            JobControlOptions = [JobControlOptions.SynchronousExecution]
        };

        string simulationId = await _simulationService.CreateAsync(create);
        Assert.NotNull(simulationId);

        var simulation = await _simulationService.GetSimulationAsync(simulationId);
        Assert.Equal("TestRunSimulation", simulation.Title);

        var createJobHandler = Services.GetRequiredService
            <IRequestHandler<CreateSimulationJobRequest, SimulationJob>>();

        var job = await createJobHandler.Handle(new CreateSimulationJobRequest
        {
            SimulationId = simulationId, Execute = new Execute()
        }, CancellationToken.None);

        Assert.Null(job.HangfireJobKey);
        Assert.NotNull(job.StartedUtc);
        Assert.NotNull(job.FinishedUtc);
        Assert.NotNull(job.ResultId);

        var result = await _resultService.FindAsync(job.ResultId);

        Assert.NotNull(result);
        Assert.NotNull(result.FeatureCollection);
        var loadedJob = await _simulationService.GetSimulationJobAsync(job.JobId);
        Assert.Null(loadedJob.HangfireJobKey);
        Assert.Equal(job.ResultId, loadedJob.ResultId);
        Assert.Equal(100, loadedJob.Progress);
        Assert.Equal(StatusCode.Successful, loadedJob.Status);
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

        var mock = SmartOpenHamburg.BackgroundMock;
        mock.Verify(x => x.Create(
            It.Is<Job>(
                acceptedJobRequest => acceptedJobRequest.Method.Name == "Send"
                                      && ((SimulationRunJobRequest)acceptedJobRequest.Args[0]).JobId == job.JobId),
            It.IsAny<EnqueuedState>()));

        Assert.Null(job.ResultId);

        var loadedJob = await _simulationService.GetSimulationJobAsync(job.JobId);
        Assert.Equal(job.ResultId, loadedJob.ResultId);
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
        await _simulationService.DeleteSimulationAsync(simulationId);
        var notFound = await _simulationService.FindSimulationAsync(simulationId);
        Assert.Null(notFound);
        await Assert.ThrowsAsync<NotFoundException>(() =>
            _simulationService.GetSimulationAsync(simulationId));
    }
}