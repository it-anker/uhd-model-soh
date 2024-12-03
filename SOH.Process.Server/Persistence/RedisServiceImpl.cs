using System.Runtime.CompilerServices;
using System.Text.Json;
using Newtonsoft.Json;
using SOH.Process.Server.Models.Processes;
using StackExchange.Redis;
using static System.ArgumentNullException;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace SOH.Process.Server.Persistence;

public class RedisServiceImpl(
    IConnectionMultiplexer redis,
    JsonSerializerSettings jsonOptions) : IPersistence
{
    private readonly IDatabaseAsync _database = redis.GetDatabase();

    public async Task UpsertAsync<TEntity>(string key, TEntity entity, CancellationToken token = default)
    {
        ThrowIfNull(entity);
        ArgumentException.ThrowIfNullOrEmpty(key);

        string json = JsonConvert.SerializeObject(entity);
        await _database.StringSetAsync(key, json);
    }

    public Task UploadFileAsync(string key, Stream stream, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public async Task<TEntity?> FindAsync<TEntity>(string key, CancellationToken token = default)
    {
        var value = await _database.StringGetAsync(key);

        return value.IsNullOrEmpty ? default : JsonConvert.DeserializeObject<TEntity>(value!, jsonOptions);
    }

    public async IAsyncEnumerable<TEntity> ListAsync<TEntity>(string query,
        [EnumeratorCancellation] CancellationToken token = default)
    {
        var keys = redis.ScanKeysAllServerAsync(query, token);

        await foreach (var redisKey in keys)
        {
            var value = await _database.StringGetAsync(redisKey);

            if (value.IsNullOrEmpty)
            {
                continue;
            }

            var entity = JsonConvert.DeserializeObject<TEntity>(value!, jsonOptions);

            if (!Equals(entity, default(TEntity)))
            {
                yield return entity!;
            }
        }
    }

    public async Task<ParameterLimitResponse<TEntity>> ListPaginatedAsync<TEntity>(string query,
        ParameterLimit parameterLimit, CancellationToken token = default)
    {
        var db = redis.GetDatabase();
        var servers = redis.GetEndPoints().Select(endpoint => redis.GetServer(endpoint));

        var keysPerServer = new List<RedisKey>();

        // Berechne globale Start- und Endposition für die Paginierung
        int globalStart = (parameterLimit.PageNumber - 1) * parameterLimit.PageSize;
        int totalKeysCount = 0;

        // Iterate over each server
        foreach (var server in servers)
        {
            // Hole alle Keys auf dem aktuellen Server (effizienter wäre ein Index)
            var serverKeys = server.Keys(pattern: query).ToArray();

            totalKeysCount += serverKeys.Length;

            // Apply the pagination for the current server, only when you have not collected enough keys.
            if (keysPerServer.Count < parameterLimit.PageSize)
            {
                int remainingCapacity = parameterLimit.PageSize - keysPerServer.Count;

                // Calculate the keys you need to retrieve from the server.
                var paginatedKeys = serverKeys
                    .Skip(globalStart) // Skip the keys until the start of the curren page.
                    .Take(remainingCapacity); // Get only as much as keys as you need

                keysPerServer.AddRange(paginatedKeys);

                // Update the starter position, if more keys are required.
                globalStart = Math.Max(0, globalStart - serverKeys.Length);
            }
        }

        // Retrieve the value from the paginated keys
        var tasks = keysPerServer.Select(key => db.StringGetAsync(key));
        var values = await Task.WhenAll(tasks);

        var data = values
            .Where(v => v.HasValue)
            .Select(v => JsonConvert.DeserializeObject<TEntity>(v.ToString())!)
            .ToList();

        return new ParameterLimitResponse<TEntity>(data, totalKeysCount, parameterLimit.PageSize,
            parameterLimit.PageNumber);
    }

    public async Task<TEntity?> DeleteAsync<TEntity>(string key, CancellationToken token = default)
    {
        var value = await _database.StringGetDeleteAsync(key);
        return value.IsNullOrEmpty ? default : JsonConvert.DeserializeObject<TEntity>(value!);
    }
}