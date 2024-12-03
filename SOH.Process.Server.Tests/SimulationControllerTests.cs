using System.Net;
using NetTopologySuite.IO.Converters;
using SOH.Process.Server.Generated;
using SOH.Process.Server.Models.Ogc;
using SOH.Process.Server.Tests.Base;
using ProblemDetails = Microsoft.AspNetCore.Mvc.ProblemDetails;
using StatusCode = SOH.Process.Server.Models.Processes.StatusCode;

namespace SOH.Process.Server.Tests;

public class SimulationControllerTests : AbstractManagementTests
{
    private readonly IJobsClient _jobsClient;
    private readonly IProcessesClient _processesClient;
    private readonly IConformanceClient _conformanceClient;
    private readonly ICapabilitiesClient _capabilitiesClient;

    public SimulationControllerTests(OgcIntegration services) : base(services)
    {
        var httpClient = SmartOpenHamburg.CreateClient();
        _jobsClient = new JobsClient(httpClient);
        _processesClient = new ProcessesClient(httpClient){ReadResponseAsString = true};
        _conformanceClient = new ConformanceClient(httpClient);
        _capabilitiesClient = new CapabilitiesClient(httpClient);
    }

    [Fact]
    public async Task TestListProcesses()
    {
        var processes = await _processesClient.GetProcessesAsync(100);

        Assert.Contains(processes.Processes, summary => summary.Title == "SOH - Ferry Transfer Model");
        Assert.Contains(processes.Processes, summary =>
            summary.Description == "Simple transfer model to of the Hamburg HADAG ferry system.");
        Assert.Contains(processes.Processes, summary => summary.Title == "TestModel");

        var ferrySimulation = processes.Processes.First(summary =>
            summary.Id == GlobalConstants.FerryTransferId);
        var ferrySimulationLoaded = await _processesClient.GetProcessDescriptionAsync(ferrySimulation.Id);
        Assert.Single(ferrySimulationLoaded.Outputs);
        Assert.Equal(2, ferrySimulationLoaded.JobControlOptions.Count);
    }

    [Fact]
    public async Task TestExceptionProcess()
    {
        var exception = await Assert.ThrowsAsync<ApiException<ProblemDetails>>(() =>
            _processesClient.GetProcessDescriptionAsync(Guid.Empty.ToString()));

        Assert.Equal((int)HttpStatusCode.NotFound, exception.StatusCode);
        Assert.NotNull(exception.Response);
    }

    [Fact]
    public async Task TestExecuteProcess()
    {
        var processes = await _processesClient.GetProcessesAsync(100);

        var testModel = processes.Processes.First(summary => summary.Title == "TestModel");

        var result = await _processesClient.ExecuteAsync(testModel.Id, new Execute());

        Assert.NotNull(result.FeatureCollection);
        Assert.Single(result.FeatureCollection);
        Assert.NotNull(result.JobId);
        Assert.Null(result.FileId);
        Assert.Equal(testModel.Id, result.ProcessId);
    }

    [Fact]
    public async Task TestExecuteFerryProcess()
    {
        var ferrySimulation = await _processesClient
            .GetProcessDescriptionAsync(GlobalConstants.FerryTransferId);

        string configContent = await File.ReadAllTextAsync("ferry_transfer_test_config_2.json");
        var result = await _processesClient.ExecuteAsync(ferrySimulation.Id, new Execute
        {
            Inputs = new Dictionary<string, object>
            {
                { "config", configContent }
            }
        });

        Assert.NotNull(result.FeatureCollection);
        Assert.NotEmpty(result.FeatureCollection);
        Assert.NotNull(result.JobId);
        Assert.Null(result.FileId);
        Assert.Equal(ferrySimulation.Id, result.ProcessId);

        var exception = await Assert.ThrowsAsync<ApiException<ProblemDetails>>(() =>
            _jobsClient.GetStatusAsync(Guid.NewGuid().ToString()));
        Assert.Equal((int)HttpStatusCode.NotFound, exception.StatusCode);

        exception = await Assert.ThrowsAsync<ApiException<ProblemDetails>>(() =>
            _jobsClient.GetResultAsync(Guid.NewGuid().ToString()));
        Assert.Equal((int)HttpStatusCode.NotFound, exception.StatusCode);

        var executedJob = await _jobsClient.GetStatusAsync(result.JobId);
        Assert.Equal(StatusCode.Successful, executedJob.Status);
        Assert.NotNull(executedJob.StartedUtc);
        Assert.NotNull(executedJob.FinishedUtc);
        Assert.Equal(100, executedJob.Progress);
        Assert.Equal(result.JobId, executedJob.JobId);
        Assert.Equal(ferrySimulation.Id, executedJob.ProcessId);
    }
}