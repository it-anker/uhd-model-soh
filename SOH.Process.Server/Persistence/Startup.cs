using System.Text.Json;
using System.Text.Json.Serialization;
using NetTopologySuite.IO.Converters;
using SOH.Process.Server.Background;
using StackExchange.Redis;

namespace SOH.Process.Server.Persistence;

public static class Startup
{
    public static IServiceCollection AddPersistence(this IServiceCollection services,
        IConfiguration configuration, IWebHostEnvironment environment)
    {
        var databaseSettings = configuration.GetSection("Redis").Get<RedisDatabaseSettings>();
        ArgumentNullException.ThrowIfNull(databaseSettings);

        var jsonOptions = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
        jsonOptions.Converters.Add(new GeoJsonConverterFactory());

        return services
            .AddSingleton(jsonOptions)
            .AddSingleton<IConnectionMultiplexer>(_ =>
                ConnectionMultiplexer.Connect(databaseSettings.ConnectionString))
            .AddBackgroundJobs(configuration, environment)
            .AddScoped<IPersistence, RedisServiceImpl>();
    }
}