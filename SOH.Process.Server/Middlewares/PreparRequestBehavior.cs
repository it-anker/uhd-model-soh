using MediatR;
using SOH.Process.Server.Models.Ogc;
using SOH.Process.Server.Simulations;

namespace SOH.Process.Server.Middlewares;

public class PrepareRequestBehavior<TRequest, TResponse>(IHttpContextAccessor contextAccessor)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (contextAccessor.HttpContext is { } httpContext &&
            httpContext.Request.Headers.TryGetValue("Prefer", out var preferredExecution) &&
            request is CreateSimulationJobRequest jobRequest)
        {
            if (preferredExecution == "respond-async")
            {
                jobRequest.Prefer = JobControlOptions.AsyncExecution;
            }
            else if (preferredExecution == "respond-sync")
            {
                jobRequest.Prefer = JobControlOptions.SynchronousExecution;
            }
        }

        return next();
    }
}