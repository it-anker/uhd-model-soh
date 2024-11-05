using Serilog;
using Serilog.Context;

namespace SOH.Process.Server.Middlewares;

public class ResponseLoggingMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        await next(context);
        Stream originalBody = context.Response.Body;
        await using MemoryStream newBody = new MemoryStream();
        context.Response.Body = newBody;
        newBody.Seek(0, SeekOrigin.Begin);
        string responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();

        LogContext.PushProperty("StatusCode", context.Response.StatusCode);
        LogContext.PushProperty("ResponseTimeUtc", DateTime.UtcNow);
        Log.ForContext("ResponseHeaders",
                context.Response.Headers.ToDictionary(h => h.Key, h => h.Value.ToString()),
                true)
            .ForContext("ResponseBody", responseBody)
            .Information(
                "HTTP {RequestMethod} Request to {RequestPath} has Status Code {StatusCode}",
                context.Request.Method, context.Request.Path, context.Response.StatusCode);

        newBody.Seek(0, SeekOrigin.Begin);
        await newBody.CopyToAsync(originalBody);
    }
}