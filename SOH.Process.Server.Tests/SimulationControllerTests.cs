using System.Net;
using NetTopologySuite.Features;
using NetTopologySuite.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SOH.Process.Server.Generated;
using SOH.Process.Server.Models.Ogc;
using SOH.Process.Server.Tests.Base;
using FeatureCollection = NetTopologySuite.Features.FeatureCollection;
using Output = SOH.Process.Server.Models.Ogc.Output;
using ProblemDetails = Microsoft.AspNetCore.Mvc.ProblemDetails;
using ResponseKind = SOH.Process.Server.Models.Ogc.ResponseKind;
using StatusCode = SOH.Process.Server.Models.Processes.StatusCode;

namespace SOH.Process.Server.Tests;

public class SimulationControllerTests : AbstractManagementTests
{
    private readonly IJobsClient _jobsClient;
    private readonly IProcessesClient _processesClient;
    private readonly IConformanceClient _conformanceClient;
    private readonly ICapabilitiesClient _capabilitiesClient;
    private readonly IResultsClient _resultsClient;

    public SimulationControllerTests(OgcIntegration services) : base(services)
    {
        var httpClient = SmartOpenHamburg.CreateClient();
        _jobsClient = new JobsClient(httpClient);
        _processesClient = new ProcessesClient(httpClient){ReadResponseAsString = true};
        _conformanceClient = new ConformanceClient(httpClient);
        _capabilitiesClient = new CapabilitiesClient(httpClient);
        _resultsClient = new ResultsClient(httpClient);
    }

    [Fact]
    public async Task TestCapabilities()
    {
        var landingPage = await _capabilitiesClient.GetLandingPageAsync();
        Equal("SmartOpenHamburg OGC Processes API", landingPage.Title);
        Equal("Official OGC Processes API to execute the SmartOpenHamburg MARS simulation",
            landingPage.Description);
    }

    [Fact]
    public async Task TestConformanceClasses()
    {
        var classes = await _conformanceClient.GetConformanceClassesAsync();

        NotEmpty(classes.ConformsTo);
        Contains(classes.ConformsTo, conformity => conformity.EndsWith("json"));
        Contains(classes.ConformsTo, conformity => conformity.EndsWith("job-list"));
    }

    [Fact]
    public async Task TestListProcesses()
    {
        var processes = await _processesClient.GetProcessesAsync(100, null);

        Contains(processes.Processes, summary => summary.Title == "SOH - Ferry Transfer Model");
        Contains(processes.Processes, summary =>
            summary.Description == "Simple transfer model to of the Hamburg HADAG ferry system.");
        Contains(processes.Processes, summary => summary.Title == "TestModel");

        var ferrySimulation = processes.Processes.First(summary =>
            summary.Id == GlobalConstants.FerryTransferId);
        var ferrySimulationLoaded = await _processesClient.GetProcessDescriptionAsync(ferrySimulation.Id);
        True(ferrySimulationLoaded.Outputs.Count >= 2);
        Equal(2, ferrySimulationLoaded.JobControlOptions.Count);
    }

    [Fact]
    public async Task TestExceptionProcess()
    {
        var exception = await ThrowsAsync<ApiException<ProblemDetails>>(() =>
            _processesClient.GetProcessDescriptionAsync(Guid.Empty.ToString()));

        Equal((int)HttpStatusCode.NotFound, exception.StatusCode);
        NotNull(exception.Response);
    }

    [Fact]
    public async Task TestLimitProcess()
    {
        var processes = await _processesClient.GetProcessesAsync(1, "testProcessId");
        Single(processes.Processes);
        Equal("simulation:testProcessId", processes.Processes[0].Id);
    }

    [Fact]
    public async Task TestExecuteProcess()
    {
        var processes = await _processesClient.GetProcessesAsync(100, null);

        var testModel = processes.Processes.First(summary => summary.Title == "TestModel");

        object executionProcess = await _processesClient.ExecuteAsync(testModel.Id, new Execute
        {
            Response = ResponseKind.Document,
            JobIdentifier = nameof(TestExecuteProcess),
            Outputs = new Dictionary<string, Output>
            {
                { "default", new Output() }
            }
        });

        string json = ((JObject)executionProcess).ToString();
        var results = JsonConvert.DeserializeObject<Results>(json);
        NotNull(results);
        True(results.ContainsKey("default"));
        object? result = results["default"];
        Assert.NotNull(result);
        string? resultJson = result.ToString();
        var featureCollection = new GeoJsonReader().Read<FeatureCollection>(resultJson);
        Single(featureCollection);

        var jobs = await _jobsClient.GetJobsAsync(null, null, "TestExecuteProcess");
        Single(jobs.Jobs);
        var job = jobs.Jobs.First(info => info.ProcessId == testModel.Id);
        Equal(StatusCode.Successful, job.Status);
        NotNull(job.FinishedUtc); ;

        var executedJob = await _jobsClient.GetStatusAsync(job.JobId);
        Equal(100, executedJob.Progress);
        Equal(testModel.Id, executedJob.ProcessId);
        Equal(StatusCode.Successful, executedJob.Status);
        NotNull(executedJob.FinishedUtc);
        StartsWith("Finished job execution", executedJob.Message);
    }

    [Fact]
    public async Task TestExecuteBikesProcess()
    {
        var bikesSimulation = await _processesClient
            .GetProcessDescriptionAsync(GlobalConstants.Green4BikesId);

        string configContent = await File.ReadAllTextAsync("green_4_bikes_test_config.json");
        object executionResponse = await _processesClient.ExecuteAsync(bikesSimulation.Id, new Execute
        {
            JobIdentifier = "bikesSingle",
            Inputs = new Dictionary<string, object>
            {
                { "config", configContent },
                { "startPoint", "2024-12-01T06:45:00" },
                { "endPoint", "2024-12-01T07:15:00" }
            },
            Outputs = new Dictionary<string, Output>
            {
                { "agents", new Output() }
            }
        });
        var geoJsonReader = new GeoJsonReader();
        var rawResult = geoJsonReader.Read<FeatureCollection>(((JObject)executionResponse).ToString());
        NotNull(rawResult);
        NotNull(rawResult);
        NotEmpty(rawResult);
    }

    [Fact]
    public async Task TestExecuteFerryProcess()
    {
        var ferrySimulation = await _processesClient
            .GetProcessDescriptionAsync(GlobalConstants.FerryTransferId);

        string configContent = await File.ReadAllTextAsync("ferry_transfer_test_config_2.json");
        object executionResponse = await _processesClient.ExecuteAsync(ferrySimulation.Id, new Execute
        {
            JobIdentifier = "ferrySingle",
            Inputs = new Dictionary<string, object>
            {
                { "config", configContent },
                { "startPoint", "2024-12-01T08:00:00" },
                { "endPoint", "2024-12-01T09:00:00" }
            },
            Outputs = new Dictionary<string, Output>
            {
                { "agents", new Output()},
                { "not_existing_output", new Output()}
            }
        });
        var geoJsonReader = new GeoJsonReader();
        var rawResult = geoJsonReader.Read<FeatureCollection>(((JObject)executionResponse).ToString());
        NotNull(rawResult);
        NotNull(rawResult);
        NotEmpty(rawResult);

        var jobs = await _jobsClient.GetJobsAsync(null, null, query: "ferrySingle");
        var job = jobs.Jobs.First(info => info.ProcessId == GlobalConstants.FerryTransferId);

        var result = await _resultsClient.GetResultByJobAsync(job.JobId);
        Null(result.FileId);
        Equal(ferrySimulation.Id, result.ProcessId);

        var exception = await ThrowsAsync<ApiException<ProblemDetails>>(() =>
            _jobsClient.GetStatusAsync(Guid.NewGuid().ToString()));
        Equal((int)HttpStatusCode.NotFound, exception.StatusCode);

        exception = await ThrowsAsync<ApiException<ProblemDetails>>(() =>
            _jobsClient.GetResultAsync(Guid.NewGuid().ToString()));
        Equal((int)HttpStatusCode.NotFound, exception.StatusCode);

        var executedJob = await _jobsClient.GetStatusAsync(result.JobId);
        Equal(StatusCode.Successful, executedJob.Status);
        NotNull(executedJob.StartedUtc);
        NotNull(executedJob.FinishedUtc);
        Equal(100, executedJob.Progress);
        Equal(result.JobId, executedJob.JobId);
        Equal(ferrySimulation.Id, executedJob.ProcessId);

        var jobResult = (JObject)await _jobsClient.GetResultAsync(executedJob.JobId);
        NotNull(jobResult);
        True(jobResult.ContainsKey("type"));
        Equal("FeatureCollection", jobResult["type"]!.ToString());

        object singleStatistics = await _processesClient.ExecuteAsync(ferrySimulation.Id, new Execute
        {
            Inputs = new Dictionary<string, object>
            {
                { "config", configContent },
                { "startPoint", "2024-12-01T08:00:00" },
                { "endPoint", "2024-12-01T09:00:00" }
            },
            Outputs = new Dictionary<string, Output>
            {
                { "soh_output_avg_road_count", new Output() }
            }
        });

        string? json = singleStatistics.ToString();
        NotNull(json);
        var series = JsonConvert.DeserializeObject<List<TimeSeriesStep>>(json);
        NotNull(series);
        Contains(series, step => step.Value > 0);

        object multiResult = await _processesClient.ExecuteAsync(ferrySimulation.Id, new Execute
        {
            Inputs = new Dictionary<string, object>
            {
                { "config", configContent },
                { "startPoint", "2024-12-01T08:00:00" },
                { "endPoint", "2024-12-01T09:00:00" }
            },
            Outputs = new Dictionary<string, Output>
            {
                { "agents", new Output()},
                { "soh_output_sum_modality_usage", new Output()},
                { "soh_output_avg_road_count", new Output() }
            }
        });
        NotNull(multiResult);
    }

    [Fact]
    public async Task TestExecuteFerryProcessDefault()
    {
        var ferrySimulation = await _processesClient
            .GetProcessDescriptionAsync(GlobalConstants.FerryTransferId);
        object executionResponse = await _processesClient.ExecuteAsync(
            GlobalConstants.FerryTransferId, new Execute
            {
                JobIdentifier = nameof(TestExecuteFerryProcessDefault),
                Outputs = new Dictionary<string, Output>
                {
                    { "agents", new Output()}
                }
            });
        var geoJsonReader = new GeoJsonReader();
        var result = geoJsonReader.Read<FeatureCollection>(((JObject)executionResponse).ToString());

        NotNull(result);
        NotEmpty(result);

        var jobs = await _jobsClient.GetJobsAsync(null, null, "FerryProcessDefa");
        Single(jobs.Jobs);
        var job = jobs.Jobs.First();
        NotNull(job.JobId);
        Equal(ferrySimulation.Id, job.ProcessId);

        var executedJob = await _jobsClient.GetStatusAsync(job.JobId);
        Equal(StatusCode.Successful, executedJob.Status);
        NotNull(executedJob.StartedUtc);
        NotNull(executedJob.FinishedUtc);
        Equal(100, executedJob.Progress);
        Equal(job.JobId, executedJob.JobId);
        Equal(ferrySimulation.Id, executedJob.ProcessId);

        var jobResult = await _jobsClient.GetResultAsync(executedJob.JobId);
        NotNull(jobResult);
    }
}