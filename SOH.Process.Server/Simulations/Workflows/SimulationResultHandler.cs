using MediatR;
using SharpCompress.Common;
using SOH.Process.Server.Models.Ogc;
using SOH.Process.Server.Models.Processes;
using Results = SOH.Process.Server.Models.Ogc.Results;

namespace SOH.Process.Server.Simulations.Workflows;

public class SimulationResultHandler(
    ISimulationService simulationService,
    IResultService resultService)
    : IRequestHandler<GetJobResultRequest, Results>
{
    public async Task<Results> Handle(GetJobResultRequest request, CancellationToken cancellationToken)
    {
        var job = await simulationService.GetSimulationJobAsync(request.JobId, cancellationToken);
        var process = await simulationService.GetSimulationAsync(job.ProcessId, cancellationToken);

        var response = new Results();
        foreach ((string? output, var definition) in process.Outputs)
        {
            var results = resultService.ListResultsAsync(
                job.JobId, output, cancellationToken);
            var firstResultOfOutput = results
                .ToBlockingEnumerable(cancellationToken: cancellationToken)
                .FirstOrDefault();

            if (firstResultOfOutput == null) continue;

            if (definition.TransmissionMode.GetValueOrDefault() == TransmissionMode.Value &&
                firstResultOfOutput.FeatureCollection != null)
            {
                response.Add(output, firstResultOfOutput.FeatureCollection);
            }

            if (definition.TransmissionMode.GetValueOrDefault() == TransmissionMode.Reference)
            {
                if (!string.IsNullOrEmpty(firstResultOfOutput.FileId))
                {
                    response.Add(output, firstResultOfOutput.FileId);
                }
                else if (firstResultOfOutput.FeatureCollection != null)
                {
                    response.Add(output, firstResultOfOutput.Id);
                }
            }
        }

        return response;
    }
}