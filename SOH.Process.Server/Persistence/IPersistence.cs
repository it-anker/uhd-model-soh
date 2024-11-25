using SOH.Process.Server.Models.Processes;

namespace SOH.Process.Server.Persistence;

public interface IPersistence
{
    Task UpsertAsync<TEntity>(string key, TEntity entity, CancellationToken token = default);
    Task<TEntity?> DeleteAsync<TEntity>(string key, CancellationToken token = default);
    Task<TEntity?> FindAsync<TEntity>(string key, CancellationToken token = default);

    IAsyncEnumerable<TEntity> ListAsync<TEntity>(string query, CancellationToken token = default);

    Task<ParameterLimitResponse<TEntity>> ListPaginatedAsync<TEntity>(
        string query, ParameterLimit parameterLimit, CancellationToken token = default);
}