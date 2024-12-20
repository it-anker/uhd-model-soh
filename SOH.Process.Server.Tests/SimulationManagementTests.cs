using Hangfire.Common;
using Hangfire.States;
using Mapster;
using Mars.Common.Core;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using SOH.Process.Server.Models.Common.Exceptions;
using SOH.Process.Server.Models.Ogc;
using SOH.Process.Server.Models.Processes;
using SOH.Process.Server.Simulations;
using SOH.Process.Server.Simulations.Jobs;
using SOH.Process.Server.Tests.Base;
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
        NotNull(simulationId);

        var simulation = await _simulationService.GetSimulationAsync(simulationId);
        Equal("my sim desc", simulation.Description);
        Equal("SOH Test", simulation.Title);
        Equal("1.0.0", simulation.Version);
        Contains("planning", simulation.Keywords);
        DoesNotContain("soh", simulation.Keywords);
        Contains("ferry", simulation.Keywords);
        Single(simulation.Inputs);
        Single(simulation.OutputTransmission);
        Equal(TransmissionMode.Value, simulation.OutputTransmission[0]);
        Equal(JobControlOptions.SynchronousExecution, simulation.JobControlOptions[0]);
        Contains("myInput", simulation.Inputs);

        var input = simulation.Inputs.Values.First();
        Equal("my input desc", input.Description);
        Equal(1, input.MinOccurs);
        Equal(2, input.MaxOccurs.Value<int>());
        Equal("MyInput", input.Title);
        Contains("param", input.Keywords);

        Single(simulation.Outputs);
        var output = simulation.Outputs.Values.First();
        Equal(TransmissionMode.Value, output.TransmissionMode);
        NotNull(output.Format);
        Equal("application/geo+json", output.Format.MediaType);

        await ThrowsAsync<NotFoundException>(() =>
            _simulationService.GetSimulationAsync(Guid.NewGuid().ToString()));

        var found = await _simulationService.FindSimulationAsync(simulationId);
        NotNull(found);
        Equal(simulationId, found.Id);
        var notFound = await _simulationService.FindSimulationAsync(Guid.NewGuid().ToString());
        Null(notFound);
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
            StartsWith("simulation", id);
        }

        var filter = new SearchProcessRequest
        {
            PageSize = 1, PageNumber = 1
        };
        var response = await _simulationService.ListProcessesAsync(filter);
        NotEmpty(response.Processes);
        Single(response.Processes);

        var paginatedResponse = await _simulationService.ListProcessesPaginatedAsync(new SearchProcessRequest
        {
            PageSize = 1, Query = ""
        });

        Single(paginatedResponse.Data);
        True(paginatedResponse.HasNextPage);
        True(paginatedResponse.TotalCount > 1);
        Equal(1, paginatedResponse.PageSize);
        Equal(1, paginatedResponse.CurrentPage);

        var paginatedNext = await _simulationService.ListProcessesPaginatedAsync(new SearchProcessRequest
        {
            PageSize = 1, Query = "", PageNumber = 2
        });

        Single(paginatedNext.Data);
        NotEqual(paginatedNext.Data[0].Id, paginatedResponse.Data[0].Id);
        Equal(2, paginatedNext.CurrentPage);

        var ferryResponse = await _simulationService.ListProcessesPaginatedAsync(new SearchProcessRequest
        {
            PageSize = 1, Query = "ferry"
        });

        Assert.Single(ferryResponse.Data);
        Assert.Equal(GlobalConstants.FerryTransferId, ferryResponse.Data[0].Id);
    }

    [Fact]
    public async Task TestUpdateSimulation()
    {
        var create = new CreateSimulationProcessDescriptionRequest
        {
            Title = "TestUpdateSimulation",
            Description = "my sim desc"
        };
        await ThrowsAsync<ArgumentNullException>(() => _simulationService.CreateAsync(create));
        create.Version = "1.0.0";
        string simulationId = await _simulationService.CreateAsync(create);
        NotNull(simulationId);

        var simulation = await _simulationService.GetSimulationAsync(simulationId);
        Equal("TestUpdateSimulation", simulation.Title);
        var update = simulation.Adapt<UpdateSimulationProcessDescriptionRequest>();
        Equal("my sim desc", simulation.Description);
        update.Description = "my updated desc";
        update.Version = "1.0.1";

        await _simulationService.UpdateAsync(simulationId, update);
        var updatedSimulation = await _simulationService.GetSimulationAsync(simulationId);

        Equal("my updated desc", updatedSimulation.Description);
        Equal("1.0.1", updatedSimulation.Version);
        await ThrowsAsync<NotFoundException>(() =>
            _simulationService.GetSimulationAsync(Guid.NewGuid().ToString()));
    }

    [Fact]
    public async Task TestDefaultSimulation()
    {
        var existingProcess = await _simulationService.ListProcessesAsync(new SearchProcessRequest
        {
            PageSize = 1000
        });
        Contains(existingProcess.Processes, summary => summary.Title?.Contains("Ferry Transfer") == true);
    }

    [Fact]
    public async Task TestRunSimulationSync()
    {
        var create = new CreateSimulationProcessDescriptionRequest
        {
            Title = "TestRunSimulation",
            Version = "1.0.0", IsTest = true,
            Description = "my sim desc",
            JobControlOptions = [JobControlOptions.SynchronousExecution]
        };

        string simulationId = await _simulationService.CreateAsync(create);
        NotNull(simulationId);

        var simulation = await _simulationService.GetSimulationAsync(simulationId);
        Equal("TestRunSimulation", simulation.Title);

        var createJobHandler = Services.GetRequiredService
            <IRequestHandler<CreateSimulationJobRequest, SimulationJob>>();

        var job = await createJobHandler.Handle(new CreateSimulationJobRequest
        {
            SimulationId = simulationId, Execute = new Execute()
        }, CancellationToken.None);

        Null(job.HangfireJobKey);
        NotNull(job.StartedUtc);
        NotNull(job.FinishedUtc);
        NotNull(job.ResultId);

        var result = await _resultService.FindAsync(job.ResultId);

        NotNull(result);
        NotEmpty(result.Results);
        Contains(result.Results, pair => pair.Value.FeatureCollection != null);
        var loadedJob = await _simulationService.GetSimulationJobAsync(job.JobId);
        Null(loadedJob.HangfireJobKey);
        Equal(job.ResultId, loadedJob.ResultId);
        Equal(100, loadedJob.Progress);
        Equal(StatusCode.Successful, loadedJob.Status);
    }

    [Fact]
    public async Task TestRunFerrySimulationSyncWithoutAgents()
    {
        var ferryTransferProcess = await _simulationService.GetSimulationAsync(GlobalConstants.FerryTransferId);

        Equal("Simple transfer model to of the Hamburg HADAG ferry system.", ferryTransferProcess.Description);
        Equal("SOH - Ferry Transfer Model", ferryTransferProcess.Title);
        NotEmpty(ferryTransferProcess.Outputs);
        var singleOutput = ferryTransferProcess.Outputs.Values.First();
        NotNull(singleOutput.Schema);
        Equal("Point-based output of each agent and their values with different simulation times.",
            singleOutput.Schema.Title);

        Contains(GlobalConstants.FerryTransfer, ferryTransferProcess.Id);
        Equal(ProcessExecutionKind.Direct, ferryTransferProcess.ExecutionKind);
        Contains("simulation", ferryTransferProcess.Keywords);

        var createJobHandler = Services.GetRequiredService
            <IRequestHandler<CreateSimulationJobRequest, SimulationJob>>();

        string configContent = await File.ReadAllTextAsync("ferry_transfer_test_config.json");
        var job = await createJobHandler.Handle(new CreateSimulationJobRequest
        {
            SimulationId = ferryTransferProcess.Id, Execute = new Execute
            {
                Outputs = new Dictionary<string, Output>
                {
                    { "agents", new Output() }
                },
                Inputs = new Dictionary<string, object>
                {
                    { "config", configContent }
                }
            }
        }, CancellationToken.None);

        Null(job.HangfireJobKey);
        NotNull(job.StartedUtc);
        NotNull(job.FinishedUtc);
        NotNull(job.ResultId);

        var result = await _resultService.FindAsync(job.ResultId);

        NotNull(result);

        Contains(result.Results, pair => pair.Value.FeatureCollection?.Count == 0);
        var loadedJob = await _simulationService.GetSimulationJobAsync(job.JobId);
        Null(loadedJob.HangfireJobKey);
        Equal(job.ResultId, loadedJob.ResultId);
        Equal(100, loadedJob.Progress);
        Equal(StatusCode.Successful, loadedJob.Status);
    }

    [Fact]
    public async Task TestRunFerrySimulationSyncWithAgents()
    {
        var ferryTransferProcess = await _simulationService.GetSimulationAsync(GlobalConstants.FerryTransferId);

        Equal("Simple transfer model to of the Hamburg HADAG ferry system.", ferryTransferProcess.Description);
        Equal("SOH - Ferry Transfer Model", ferryTransferProcess.Title);
        NotEmpty(ferryTransferProcess.Outputs);
        var singleOutput = ferryTransferProcess.Outputs.Values.First();
        NotNull(singleOutput.Schema);
        Equal("Point-based output of each agent and their values with different simulation times.",
            singleOutput.Schema.Title);

        Contains(GlobalConstants.FerryTransfer, ferryTransferProcess.Id);
        Equal(ProcessExecutionKind.Direct, ferryTransferProcess.ExecutionKind);
        Contains("simulation", ferryTransferProcess.Keywords);

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
                },
                Outputs = new Dictionary<string, Output>
                {
                    { "agents", new Output()}
                }
            }
        }, CancellationToken.None);

        Null(job.HangfireJobKey);
        NotNull(job.StartedUtc);
        NotNull(job.FinishedUtc);
        NotNull(job.ResultId);

        var result = await _resultService.FindAsync(job.ResultId);

        NotNull(result);
        Contains(result.Results, pair => pair.Value.FeatureCollection != null);
        // Assert.NotEmpty(result.FeatureCollection);
        // Assert.Contains(result.FeatureCollection, feature =>
        //     feature.Attributes["ActiveCapability"].ToString() == "Walking");
        var loadedJob = await _simulationService.GetSimulationJobAsync(job.JobId);
        Null(loadedJob.HangfireJobKey);
        Equal(job.ResultId, loadedJob.ResultId);
        Equal(100, loadedJob.Progress);
        Equal(StatusCode.Successful, loadedJob.Status);
    }

    [Fact]
    public async Task TestFailedJsonSimulation()
    {
        var ferryTransferProcess = await _simulationService.GetSimulationAsync(GlobalConstants.FerryTransferId);
        var createJobHandler = Services.GetRequiredService
            <IRequestHandler<CreateSimulationJobRequest, SimulationJob>>();

        await ThrowsAsync<BadRequestException>(() => createJobHandler.Handle(new CreateSimulationJobRequest
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
            IsTest = true,
            Description = "my failed sim",
            JobControlOptions = [JobControlOptions.SynchronousExecution]
        };

        string simulationId = await _simulationService.CreateAsync(create);
        NotNull(simulationId);

        var createJobHandler = Services.GetRequiredService
            <IRequestHandler<CreateSimulationJobRequest, SimulationJob>>();

        var job = await createJobHandler.Handle(new CreateSimulationJobRequest
        {
            SimulationId = simulationId, Execute = new Execute
            {
                Inputs = new Dictionary<string, object>
                {
                    { "errorInSim", true }
                }
            }
        }, CancellationToken.None);
        Equal(StatusCode.Failed, job.Status);
        NotNull(job.ExceptionMessage);
        Null(job.ResultId);
        NotNull(job.FinishedUtc);
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
        NotNull(simulationId);

        var simulation = await _simulationService.GetSimulationAsync(simulationId);
        Equal("TestRunSimulationAsync", simulation.Title);

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

        Null(job.ResultId);

        var loadedJob = await _simulationService.GetSimulationJobAsync(job.JobId);
        Equal(job.ResultId, loadedJob.ResultId);
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
        NotNull(simulationId);

        var found = await _simulationService.FindSimulationAsync(simulationId);
        NotNull(found);
        await _simulationService.DeleteAsync(simulationId);
        var notFound = await _simulationService.FindSimulationAsync(simulationId);
        Null(notFound);
        await ThrowsAsync<NotFoundException>(() =>
            _simulationService.GetSimulationAsync(simulationId));
    }
}