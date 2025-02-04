using SOH.Process.Server.Models.Ogc;

namespace SOH.Process.Server.Simulations;

public interface IResultService
{
    Task<string> CreateAsync(Result results, CancellationToken token = default);

    Task UpdateAsync(string resultId, Result request, CancellationToken token = default);

    Task DeleteAsync(string resultId, CancellationToken token = default);

    Task<Result> GetAsync(string resultId, CancellationToken token = default);

    Task<Result?> FindAsync(string resultId, CancellationToken token = default);

    IAsyncEnumerable<Result> ListResultsAsync(string jobId, string outputName,
        CancellationToken token = default);
}