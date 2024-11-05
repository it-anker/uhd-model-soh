using System.Linq.Expressions;
using Hangfire;

namespace SOH.Process.Server.Background;

public class HangfireService(
    IBackgroundJobClientV2 jobClientV2,
    IRecurringJobManagerV2 recurringJobV2)
    : IJobService, IDisposable
{
    static HangfireService()
    {
        GlobalConfiguration.Configuration
            .UseSimpleAssemblyNameTypeSerializer();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public bool Delete(string jobId)
    {
        recurringJobV2.RemoveIfExists(jobId);
        return jobClientV2.Delete(jobId);
    }

    public string Enqueue(Expression<Func<Task>> methodCall)
    {
        return jobClientV2.Enqueue(methodCall);
    }

    public string Schedule(Expression<Func<Task>> methodCall, TimeSpan delay)
    {
        return jobClientV2.Schedule(methodCall, delay);
    }

    public string Schedule(Expression<Func<Task>> methodCall, DateTimeOffset enqueueAt)
    {
        return jobClientV2.Schedule(methodCall, enqueueAt);
    }

    protected virtual void Dispose(bool disposing)
    {
        jobClientV2.Storage?
            .GetConnection()?
            .Dispose();

        recurringJobV2.Storage?
            .GetConnection()?
            .Dispose();
    }
}