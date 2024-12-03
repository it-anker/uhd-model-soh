using SOH.Process.Server.Models.Processes;
using SOH.Process.Server.Simulations.Jobs;

namespace SOH.Process.Server.Simulations.Workflows;

public class JobSubscribeWorkflow(
    ISimulationService simulationService,
    IHttpClientFactory httpClientFactory,
    IResultService resultService)
{
    public async Task NotifySubscriberAsync(SimulationRunJobRequest request, string jobId, CancellationToken token)
    {
        var currentJob = await simulationService.FindJobAsync(jobId, token);
        if (currentJob != null && request.Execute?.Subscriber != null)
        {
            var result = await resultService.FindAsync(currentJob.ResultId ?? string.Empty, token);
            using var httpClient = httpClientFactory.CreateClient();

            var subcriber = request.Execute.Subscriber;
            if (currentJob.Status == StatusCode.Failed && !string.IsNullOrEmpty(subcriber.FailedUri))
            {

            }

            if (currentJob.Status == StatusCode.Successful && !string.IsNullOrEmpty(subcriber.SuccessUri))
            {

            }

            if (currentJob.Status == StatusCode.Successful && !string.IsNullOrEmpty(subcriber.InProgressUri))
            {

            }
        }
    }
}