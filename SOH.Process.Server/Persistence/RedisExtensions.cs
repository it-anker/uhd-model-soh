using System.Runtime.CompilerServices;
using StackExchange.Redis;

namespace SOH.Process.Server.Persistence;

public static class RedisExtensions
{
    public static async IAsyncEnumerable<RedisKey> ScanKeysAllServerAsync(
        this IConnectionMultiplexer redis, string pattern,
        [EnumeratorCancellation] CancellationToken token = default)
    {
        var endpoints = redis.GetEndPoints();

        foreach (var endpoint in endpoints)
        {
            var server = redis.GetServer(endpoint);

            if (server is { IsConnected: true, IsReplica: false })
            {
                var keys = server.KeysAsync(pattern: pattern, pageSize: 100);

                await foreach (var key in keys)
                {
                    if (token.IsCancellationRequested)
                    {
                        yield break;
                    }

                    yield return key;
                }
            }
        }
    }
}