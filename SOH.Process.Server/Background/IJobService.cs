using System.Linq.Expressions;

namespace SOH.Process.Server.Background;

public interface IJobService
{
    string Enqueue(Expression<Func<Task>> methodCall);

    string Schedule(Expression<Func<Task>> methodCall, TimeSpan delay);

    string Schedule(Expression<Func<Task>> methodCall, DateTimeOffset enqueueAt);

    bool Delete(string jobId);
}