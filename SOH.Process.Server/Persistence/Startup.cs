using SOH.Process.Server.Background;
using StackExchange.Redis;

namespace SOH.Process.Server.Persistence;

public static class Startup
{
    public static IServiceCollection AddPersistence(this IServiceCollection services,
        IConfiguration configuration, IWebHostEnvironment environment)
    {
        RedisDatabaseSettings? databaseSettings = configuration.GetSection("Redis").Get<RedisDatabaseSettings>();

        ArgumentNullException.ThrowIfNull(databaseSettings);

        return services
            .AddSingleton<IConnectionMultiplexer>(_ =>
                ConnectionMultiplexer.Connect(databaseSettings.ConnectionString))
            .AddBackgroundJobs(configuration, environment)
            .AddScoped<IPersistence, RedisServiceImpl>();
    }
}