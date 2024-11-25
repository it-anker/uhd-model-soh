using SOH.Process.Server.Models.Ogc;
using SOH.Process.Server.Persistence;
using Results = SOH.Process.Server.Models.Ogc.Results;

namespace SOH.Process.Server.Simulations.Services;

public class ResultsServiceImpl(IPersistence persistence) : IResultService
{
    public async Task<string> CreateAsync(Result results, CancellationToken token = default)
    {
        string id = "result:" + Guid.NewGuid();
        var result = new Result
        {
            SimulationId = results.SimulationId, Id = id
        };

        await persistence.UpsertAsync(id, result, token);

        return id;
    }

    public Task UpdateAsync(string resultId, Result request, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(string resultId, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result> GetAsync(string resultId, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result?> FindAsync(string resultId, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public Task<Results> ListAsync(CancellationToken token = default)
    {
        throw new NotImplementedException();
    }
}