using MediatR;
using SOH.Process.Server.Models.Ogc;

namespace SOH.Process.Server.Simulations.Workflows;

public class SimulationResultHandler(SimulationResultWorkflow resultWorkflow)
    : IRequestHandler<GetJobResultRequest, JobResultResponse>
{
    public async Task<JobResultResponse> Handle(GetJobResultRequest request, CancellationToken cancellationToken)
    {
        return await resultWorkflow.RetrieveResultsAsync(request, cancellationToken);
    }
}