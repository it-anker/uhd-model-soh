using System.Text.Json;
using System.Text.Json.Serialization;
using NetTopologySuite.IO.Converters;
using Newtonsoft.Json;
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

        var SerializerSettings = new JsonSerializerSettings();

        SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
        SerializerSettings.DefaultValueHandling = DefaultValueHandling.Include;
        SerializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
        SerializerSettings.Converters.Add(new FeatureCollectionConverter());
        SerializerSettings.Converters.Add(new FeatureConverter());
        SerializerSettings.Converters.Add(new GeometryConverter());

        return services
            .AddSingleton(SerializerSettings)
            .AddSingleton<IConnectionMultiplexer>(_ =>
                ConnectionMultiplexer.Connect(databaseSettings.ConnectionString))
            .AddBackgroundJobs(configuration, environment)
            .AddScoped<IPersistence, RedisServiceImpl>();
    }
}