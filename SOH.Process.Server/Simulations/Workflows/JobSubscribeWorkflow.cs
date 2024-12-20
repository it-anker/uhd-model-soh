using Mapster;
using Newtonsoft.Json;
using SOH.Process.Server.Models.Ogc;
using SOH.Process.Server.Models.Processes;

namespace SOH.Process.Server.Simulations.Workflows;

public class JobSubscribeWorkflow(
    SimulationResultWorkflow resultWorkflow,
    ISimulationService simulationService,
    IHttpClientFactory httpClientFactory)
{
    public async Task NotifySubscriberAsync(string jobId, CancellationToken token)
    {
        var currentJob = await simulationService.FindJobAsync(jobId, token);
        if (currentJob != null)
        {
            await NotifySubscribersForJobAsync(currentJob, token);
        }
    }

    public async Task NotifySubscribersForJobAsync(SimulationJob currentJob, CancellationToken token)
    {
        if (currentJob.ExecutionConfig.Subscriber == null) return;

        using var httpClient = httpClientFactory.CreateClient();
        var subscriber = currentJob.ExecutionConfig.Subscriber;
        await NotifyFailedRun(currentJob, subscriber, httpClient, token);
        await NotifySuccessRun(currentJob, subscriber, httpClient, token);
        await NotifyProgress(currentJob, subscriber, httpClient, token);
    }

    private static async Task NotifyProgress(SimulationJob currentJob, Subscriber subcriber,
        HttpClient httpClient, CancellationToken token)
    {
        if (currentJob.Status == StatusCode.Running && !string.IsNullOrEmpty(subcriber.InProgressUri) &&
            Uri.TryCreate(subcriber.FailedUri, UriKind.Absolute, out var progressUri) &&
            (progressUri.Scheme == Uri.UriSchemeHttp || progressUri.Scheme == Uri.UriSchemeHttps))
        {
            var status = currentJob.Adapt<StatusInfo>();
            string json = JsonConvert.SerializeObject(status);
            var jsonContent = new StringContent(json);
            await httpClient.PostAsync(subcriber.InProgressUri, jsonContent, token);
        }
    }

    private async Task NotifySuccessRun(SimulationJob currentJob,
        Subscriber subscriber, HttpClient httpClient, CancellationToken token)
    {
        if (currentJob.Status == StatusCode.Successful && !string.IsNullOrEmpty(subscriber.SuccessUri) &&
            Uri.TryCreate(subscriber.FailedUri, UriKind.Absolute, out var successUri) &&
            (successUri.Scheme == Uri.UriSchemeHttp || successUri.Scheme == Uri.UriSchemeHttps))
        {
            var result = await resultWorkflow.RetrieveResultsAsync(new GetJobResultRequest
                { JobId = currentJob.JobId }, token);

            if (result.RawSingleOutput != null)
            {
                string json = JsonConvert.SerializeObject(result.RawSingleOutput);
                var jsonContent = new StringContent(json);
                await httpClient.PostAsync(subscriber.SuccessUri, jsonContent, token);
            }
        }
    }

    private static async Task NotifyFailedRun(SimulationJob currentJob, Subscriber subcriber,
        HttpClient httpClient, CancellationToken token)
    {
        if (currentJob.Status == StatusCode.Failed && !string.IsNullOrEmpty(subcriber.FailedUri) &&
            !string.IsNullOrEmpty(currentJob.ExceptionMessage) &&
            Uri.TryCreate(subcriber.FailedUri, UriKind.Absolute, out var outUri) &&
            (outUri.Scheme == Uri.UriSchemeHttp || outUri.Scheme == Uri.UriSchemeHttps))
        {
            var exception = new { exceptionMessage = currentJob.ExceptionMessage };
            string json = JsonConvert.SerializeObject(exception);
            await httpClient.PostAsync(subcriber.FailedUri, new StringContent(json), token);
        }
    }
}