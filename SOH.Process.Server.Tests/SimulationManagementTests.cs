using Hangfire.Common;
using Hangfire.States;
using Mapster;
using Mars.Common.Core;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using ServiceStack;
using SOH.Process.Server.Models.Common.Exceptions;
using SOH.Process.Server.Models.Processes;
using SOH.Process.Server.Simulations;
using SOH.Process.Server.Simulations.Jobs;
using SOH.Process.Server.Tests.Base;
using ZstdSharp.Unsafe;
using Execute = SOH.Process.Server.Models.Ogc.Execute;
using Format = SOH.Process.Server.Models.Parameters.Format;
using InputDescription = SOH.Process.Server.Models.Parameters.InputDescription;
using JobControlOptions = SOH.Process.Server.Models.Ogc.JobControlOptions;
using OutputDescription = SOH.Process.Server.Models.Ogc.OutputDescription;
using Schema = SOH.Process.Server.Models.Ogc.Schema;
using StatusCode = SOH.Process.Server.Models.Processes.StatusCode;
using TransmissionMode = SOH.Process.Server.Models.Ogc.TransmissionMode;

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
        var create = new CreateSimulationProcessDescriptionRequest
        {
            Title = "SOH Test",
            Description = "my sim desc",
            Keywords = ["planning", "ferry"],
            Version = "1.0.0",
            JobControlOptions = [JobControlOptions.SynchronousExecution],
            OutputTransmission = [TransmissionMode.Value],
            Inputs = new Dictionary<string, InputDescription>
            {
                {
                    "myInput", new InputDescription
                    {

                        Description = "my input desc", Keywords = ["input", "param"],
                        Title = "MyInput", MaxOccurs = 2, Schema = new Schema
                        {
                            Description = "my schema desc"
                        }
                    }
                }
            },
            Outputs = new Dictionary<string, OutputDescription>
            {
                {
                    "myGeoJsonOutput", new OutputDescription
                    {
                        TransmissionMode = TransmissionMode.Value,
                        Format = new Format
                        {
                            MediaType = "application/geo+json"
                        }
                    }
                }
            }
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
        Assert.Single(simulation.Inputs);
        Assert.Single(simulation.OutputTransmission);
        Assert.Equal(TransmissionMode.Value, simulation.OutputTransmission[0]);
        Assert.Equal(JobControlOptions.SynchronousExecution, simulation.JobControlOptions[0]);
        Assert.Contains("myInput", simulation.Inputs);

        var input = simulation.Inputs.Values.First();
        Assert.Equal("my input desc", input.Description);
        Assert.Equal(1, input.MinOccurs);
        Assert.Equal(2, input.MaxOccurs.Value<int>());
        Assert.Equal("MyInput", input.Title);
        Assert.Contains("param", input.Keywords);

        Assert.Single(simulation.Outputs);
        var output = simulation.Outputs.Values.First();
        Assert.Equal(TransmissionMode.Value, output.TransmissionMode);
        Assert.NotNull(output.Format);
        Assert.Equal("application/geo+json", output.Format.MediaType);

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
            var create = new CreateSimulationProcessDescriptionRequest
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
        var create = new CreateSimulationProcessDescriptionRequest
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
        var update = simulation.Adapt<UpdateSimulationProcessDescriptionRequest>();
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
        var create = new CreateSimulationProcessDescriptionRequest
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
    public async Task TestRunFerrySimulationSyncWithoutAgents()
    {
        var ferryTransferProcess = await _simulationService.GetSimulationAsync(GlobalConstants.FerryTransferId);

        Assert.Equal("Simple transfer model to of the Hamburg HADAG ferry system.", ferryTransferProcess.Description);
        Assert.Equal("SOH - Ferry Transfer Model", ferryTransferProcess.Title);
        Assert.Single(ferryTransferProcess.Outputs);
        var singleOutput = ferryTransferProcess.Outputs.Values.First();
        Assert.NotNull(singleOutput.Schema);
        Assert.Equal("Point-based output of each agent and their values with different simulation times.",
            singleOutput.Schema.Title);

        Assert.Contains(GlobalConstants.FerryTransfer, ferryTransferProcess.Id);
        Assert.Equal(ProcessExecutionKind.Direct, ferryTransferProcess.ExecutionKind);
        Assert.Contains("simulation", ferryTransferProcess.Keywords);

        var createJobHandler = Services.GetRequiredService
            <IRequestHandler<CreateSimulationJobRequest, SimulationJob>>();

        string configContent = await File.ReadAllTextAsync("ferry_transfer_test_config.json");
        var job = await createJobHandler.Handle(new CreateSimulationJobRequest
        {
            SimulationId = ferryTransferProcess.Id, Execute = new Execute
            {
                Inputs = new Dictionary<string, object>
                {
                    { "config", configContent }
                }
            }
        }, CancellationToken.None);

        Assert.Null(job.HangfireJobKey);
        Assert.NotNull(job.StartedUtc);
        Assert.NotNull(job.FinishedUtc);
        Assert.NotNull(job.ResultId);

        var result = await _resultService.FindAsync(job.ResultId);

        Assert.NotNull(result);
        Assert.NotNull(result.FeatureCollection);
        Assert.Empty(result.FeatureCollection);
        var loadedJob = await _simulationService.GetSimulationJobAsync(job.JobId);
        Assert.Null(loadedJob.HangfireJobKey);
        Assert.Equal(job.ResultId, loadedJob.ResultId);
        Assert.Equal(100, loadedJob.Progress);
        Assert.Equal(StatusCode.Successful, loadedJob.Status);
    }

    [Fact]
    public async Task TestRunFerrySimulationSyncWithAgents()
    {
        var ferryTransferProcess = await _simulationService.GetSimulationAsync(GlobalConstants.FerryTransferId);

        Assert.Equal("Simple transfer model to of the Hamburg HADAG ferry system.", ferryTransferProcess.Description);
        Assert.Equal("SOH - Ferry Transfer Model", ferryTransferProcess.Title);
        Assert.Single(ferryTransferProcess.Outputs);
        var singleOutput = ferryTransferProcess.Outputs.Values.First();
        Assert.NotNull(singleOutput.Schema);
        Assert.Equal("Point-based output of each agent and their values with different simulation times.",
            singleOutput.Schema.Title);

        Assert.Contains(GlobalConstants.FerryTransfer, ferryTransferProcess.Id);
        Assert.Equal(ProcessExecutionKind.Direct, ferryTransferProcess.ExecutionKind);
        Assert.Contains("simulation", ferryTransferProcess.Keywords);

        var createJobHandler = Services.GetRequiredService
            <IRequestHandler<CreateSimulationJobRequest, SimulationJob>>();

        string configContent = await File.ReadAllTextAsync("ferry_transfer_test_config_2.json");
        var job = await createJobHandler.Handle(new CreateSimulationJobRequest
        {
            SimulationId = ferryTransferProcess.Id, Execute = new Execute
            {
                Inputs = new Dictionary<string, object>
                {
                    { "config", configContent }
                }
            }
        }, CancellationToken.None);

        Assert.Null(job.HangfireJobKey);
        Assert.NotNull(job.StartedUtc);
        Assert.NotNull(job.FinishedUtc);
        Assert.NotNull(job.ResultId);

        var result = await _resultService.FindAsync(job.ResultId);

        Assert.NotNull(result);
        Assert.NotNull(result.FeatureCollection);
        // Assert.NotEmpty(result.FeatureCollection);
        // Assert.Contains(result.FeatureCollection, feature =>
        //     feature.Attributes["ActiveCapability"].ToString() == "Walking");
        var loadedJob = await _simulationService.GetSimulationJobAsync(job.JobId);
        Assert.Null(loadedJob.HangfireJobKey);
        Assert.Equal(job.ResultId, loadedJob.ResultId);
        Assert.Equal(100, loadedJob.Progress);
        Assert.Equal(StatusCode.Successful, loadedJob.Status);
    }

    [Fact]
    public async Task TestFailedJsonSimulation()
    {
        var ferryTransferProcess = await _simulationService.GetSimulationAsync(GlobalConstants.FerryTransferId);
        var createJobHandler = Services.GetRequiredService
            <IRequestHandler<CreateSimulationJobRequest, SimulationJob>>();

        await Assert.ThrowsAsync<BadRequestException>(() => createJobHandler.Handle(new CreateSimulationJobRequest
            {
                SimulationId = ferryTransferProcess.Id, Execute = new Execute
                {
                    Inputs = new Dictionary<string, object>
                    {
                        { "config", "invalid json" }
                    }
                }
            }, CancellationToken.None));
    }

    [Fact]
    public async Task TestFailedSimulation()
    {
        var create = new CreateSimulationProcessDescriptionRequest
        {
            Title = "TestFailedSimulationAsync",
            Version = "1.0.0",
            Description = "my failed sim",
            JobControlOptions = [JobControlOptions.SynchronousExecution]
        };

        string simulationId = await _simulationService.CreateAsync(create);
        Assert.NotNull(simulationId);

        var createJobHandler = Services.GetRequiredService
            <IRequestHandler<CreateSimulationJobRequest, SimulationJob>>();

        var job = await createJobHandler.Handle(new CreateSimulationJobRequest
        {
            SimulationId = simulationId, Execute = new Execute
            {
                Inputs = new Dictionary<string, object>
                {
                    {
                        "func", new Action<int, SimulationJob>((i, job) =>
                            throw new InvalidOperationException("any error during sim run"))
                    }
                }
            }
        }, CancellationToken.None);
        Assert.Equal(StatusCode.Failed, job.Status);
        Assert.Null(job.ResultId);
        Assert.NotNull(job.FinishedUtc);
    }

    [Fact]
    public async Task TestRunSimulationAsync()
    {
        var create = new CreateSimulationProcessDescriptionRequest
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
        var create = new CreateSimulationProcessDescriptionRequest
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