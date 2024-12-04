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
        string connectionString = configuration["REDIS_CONNECTION"] ?? databaseSettings.ConnectionString;
        var serializerSettings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Include,
            DateFormatHandling = DateFormatHandling.IsoDateFormat
        };

        serializerSettings.Converters.Add(new FeatureCollectionConverter());
        serializerSettings.Converters.Add(new FeatureConverter());
        serializerSettings.Converters.Add(new AttributesTableConverter());
        serializerSettings.Converters.Add(new GeometryConverter());

        var options = new JsonSerializerOptions
        {
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals
        };

        options.Converters.Add(new GeoJsonConverterFactory());

        return services
            .AddSingleton(options)
            .AddSingleton<IConnectionMultiplexer>(_ =>
                ConnectionMultiplexer.Connect(connectionString))
            .AddBackgroundJobs(configuration, environment)
            .AddScoped<IPersistence, RedisServiceImpl>();
    }
}