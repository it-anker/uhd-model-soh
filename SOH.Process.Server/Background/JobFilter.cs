using Hangfire.Client;
using Hangfire.Logging;

namespace SOH.Process.Server.Background;

public class JobFilter(IServiceProvider services) : IClientFilter
{
    private static readonly ILog Logger = LogProvider.GetCurrentClassLogger();

    public void OnCreating(CreatingContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        Logger.InfoFormat("Set TenantId and UserId parameters to job {0}.{1}...",
            context.Job.Method.ReflectedType?.FullName, context.Job.Method.Name);

        using IServiceScope scope = services.CreateScope();

        HttpContext? httpContext = scope.ServiceProvider.GetRequiredService<IHttpContextAccessor>().HttpContext;
        _ = httpContext ?? throw new InvalidOperationException("Can't create a TenantJob without HttpContext.");
    }

    public void OnCreated(CreatedContext context)
    {
        Logger.InfoFormat("Job created with parameters {0}",
            context.Parameters.Select(x => x.Key + "=" + x.Value)
                .Aggregate((s1, s2) => s1 + ";" + s2));
    }
}