using Microsoft.Extensions.Localization;
using SOH.Process.Server.Models.Common.Exceptions;
using SOH.Process.Server.Models.Ogc;
using SOH.Process.Server.Persistence;
using SOH.Process.Server.Resources;
using Results = SOH.Process.Server.Models.Ogc.Results;

namespace SOH.Process.Server.Simulations.Services;

public class ResultsServiceImpl(
    IStringLocalizer<SharedResource> localization,
    IPersistence persistence) : IResultService
{
    public async Task<string> CreateAsync(Result results, CancellationToken token = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(results.ProcessId);
        ArgumentException.ThrowIfNullOrEmpty(results.JobId);
        string outputKeys = string.Join(":", results.Results.Keys);
        results.Id = $"result:{results.JobId}:{results.ProcessId}{Guid.NewGuid()}:{outputKeys}".Trim(':');
        await persistence.UpsertAsync(results.Id, results, token);
        return results.Id;
    }

    public async Task UpdateAsync(string resultId, Result request, CancellationToken token = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(request.ProcessId);
        ArgumentException.ThrowIfNullOrEmpty(request.JobId);

        await persistence.UpsertAsync(resultId, request, token);
    }

    public async Task DeleteAsync(string resultId, CancellationToken token = default)
    {
        await persistence.DeleteAsync<Result>(resultId, token);
    }

    public async Task<Result> GetAsync(string resultId, CancellationToken token = default)
    {
        return await persistence.FindAsync<Result>(resultId, token) ?? throw new NotFoundException(
            localization[$"result with id {persistence} could not be found"]);
    }

    public async Task<Result?> FindAsync(string resultId, CancellationToken token = default)
    {
        return await persistence.FindAsync<Result>(resultId, token);
    }

    public async Task<Results> ListAsync(CancellationToken token = default)
    {
        var response = persistence
            .ListAsync<Result>("result*", token);

        var results = new Results();

        await foreach (var pair in response)
        {
            results.Add(pair.Id, pair);
        }

        return results;
    }

    public IAsyncEnumerable<Result> ListResultsAsync(string jobId, string outputName,
        CancellationToken token = default)
    {
        return persistence.ListAsync<Result>($"*{jobId}*{outputName}*", token);
    }
}