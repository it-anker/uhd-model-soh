using System.IO.Pipelines;

namespace SOH.Process.Server.Middlewares;

public class StreamingFileUploadMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.ContentType != null && !context.Request.ContentType.StartsWith("multipart/"))
        {
            await next(context);
            return;
        }

        var pipeReader = PipeReader.Create(context.Request.Body);
        context.Items["PipeReader"] = pipeReader;

        await next(context);
    }
}
