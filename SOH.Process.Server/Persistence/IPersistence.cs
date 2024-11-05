namespace SOH.Process.Server.Persistence;

public interface IPersistence
{
    Task UpsertAsync<TEntity>(string key, TEntity entity, CancellationToken token = default);
    Task<TEntity?> DeleteAsync<TEntity>(string key, CancellationToken token = default);
    Task<TEntity?> FindAsync<TEntity>(string key, CancellationToken token = default);
}