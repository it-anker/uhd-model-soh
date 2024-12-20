using SOH.Process.Server.Models.Ogc;
using Results = SOH.Process.Server.Models.Ogc.Results;

namespace SOH.Process.Server.Simulations.Workflows;

public class SimulationResultWorkflow(
    ISimulationService simulationService,
    IResultService resultService)
{
    public async Task<JobResultResponse> RetrieveResultsAsync(GetJobResultRequest request, CancellationToken token = default)
    {
        var job = await simulationService.GetSimulationJobAsync(request.JobId, token);
        var jobResultResponse = new JobResultResponse();
        var multiResultIds = new List<string>();
        var multiResult = new List<(string, object?)>();
        foreach ((string? output, var outputKind) in job.ExecutionConfig.Outputs)
        {
            var results = resultService.ListResultsAsync(job.JobId, output, token);
            var resultList = results
                .ToBlockingEnumerable(cancellationToken: token);
            var firstResultOfOutput = resultList.FirstOrDefault();

            if (firstResultOfOutput != null)
            {
                if (outputKind.TransmissionMode.GetValueOrDefault() == TransmissionMode.Reference)
                {
                    multiResultIds.Add(firstResultOfOutput.Id);
                }
                else if (firstResultOfOutput.Results.TryGetValue(output, out var rawResult))
                {
                    multiResult.Add((output, rawResult.GetValue()));
                }
            }
        }

        if (job.ExecutionConfig.Response.GetValueOrDefault() == ResponseKind.Document)
        {
            var results = new Results();
            foreach (var tuple in multiResult.DistinctBy(tuple => tuple.Item1))
            {
                results.Add(tuple.Item1, tuple.Item2);
            }
            jobResultResponse.DocumentOutput = results;
        }
        else if (multiResult.Count > 1)
        {
            jobResultResponse.RawMultiOutput = multiResult.ConvertAll(tuple => tuple.Item2);
        }
        else if (multiResult.Count == 1)
        {
            jobResultResponse.RawSingleOutput = multiResult[0].Item2;
        }
        else if (multiResultIds.Count > 0)
        {
            jobResultResponse.RawReferences = multiResultIds;
        }

        return jobResultResponse;
    }
}