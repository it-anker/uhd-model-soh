using Hangfire;
using Hangfire.MemoryStorage;
using Serilog;
using ILogger = Serilog.ILogger;

namespace SOH.Process.Server.Background;

public static class Startup
{
    private static readonly ILogger Logger = Log.ForContext(typeof(Startup));

    public static IServiceCollection AddBackgroundJobs(this IServiceCollection services,
        IConfiguration config, IWebHostEnvironment environment)
    {
        services
            .AddSingleton<IJobService, HangfireService>()
            .AddSingleton<JobActivator, OgcJobActivator>();

        if (!environment.IsEnvironment("Test"))
        {
            Logger.Information("Add Hangfire scheduling and background job engine");

            services.AddHangfire((_, hangfireConfig) =>
            {
                hangfireConfig
                    .UseSerilogLogProvider()
                    .UseMemoryStorage()
                    .UseFilter(new LogJobFilter())
                    .UseMediatR();
            }).AddHangfireServer(options => config.GetSection("HangfireSettings:Server").Bind(options));
        }

        return services;
    }
}