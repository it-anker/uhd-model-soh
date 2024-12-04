using MediatR;

namespace SOH.Process.Server.Middlewares;

public class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        const string prefix = nameof(LoggingBehavior<TRequest, TResponse>);

        logger.LogInformation("[{Prefix}] Handle request={XRequestData} and response={XResponseData}",
            prefix, typeof(TRequest).Name, typeof(TResponse).Name);

        var response = await next();

        logger.LogInformation("[{Prefix}] Handled {XRequestData}", prefix, typeof(TRequest).Name);
        return response;
    }
}