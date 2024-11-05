using StackExchange.Redis;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace SOH.Process.Server.Persistence;

public class RedisServiceImpl(IConnectionMultiplexer redis) : IPersistence
{
    private readonly IDatabase _database = redis.GetDatabase();

    public async Task UpsertAsync<TEntity>(string key, TEntity entity, CancellationToken token = default)
    {
        string json = JsonSerializer.Serialize(entity);
        await _database.StringSetAsync(key, json);
    }

    public async Task<TEntity?> FindAsync<TEntity>(string key, CancellationToken token = default)
    {
        RedisValue json = await _database.StringGetAsync(key);
        if (json.IsNullOrEmpty)
            return default;

        return JsonSerializer.Deserialize<TEntity>(json!);
    }

    public async Task<TEntity?> DeleteAsync<TEntity>(string key, CancellationToken token = default)
    {
        RedisValue json = await _database.StringGetDeleteAsync(key);
        if (json.IsNullOrEmpty)
            return default;

        return JsonSerializer.Deserialize<TEntity>(json!);
    }
}
