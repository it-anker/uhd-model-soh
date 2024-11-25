using System.Text;
using Microsoft.IdentityModel.Logging;
using Serilog;

namespace SOH.Process.Server.Logging;

public static class Startup
{
    public static void UseCustomSerilog(this WebApplicationBuilder builder, IWebHostEnvironment env)
    {
        IdentityModelEventSource.ShowPII = true;

        builder.Host.UseSerilog((context, _, loggerConfiguration) =>
        {
            var logOptions = context.Configuration.GetSection("Logging").Get<LogOptions>();

            ArgumentNullException.ThrowIfNull(logOptions);
            loggerConfiguration.ReadFrom.Configuration(context.Configuration);

            if (logOptions.File is { Enabled: true }) UseFileLoggingSink(env, logOptions, loggerConfiguration);
        });
    }

    private static void UseFileLoggingSink(IWebHostEnvironment env, LogOptions logOptions,
        LoggerConfiguration loggerConfiguration)
    {
        ArgumentNullException.ThrowIfNull(logOptions.File);
        string root = env.ContentRootPath;
        Directory.CreateDirectory(Path.Combine(root, "logs"));

        string path = string.IsNullOrWhiteSpace(logOptions.File.Path) ? "logs/.txt" : logOptions.File.Path;
        if (!Enum.TryParse(logOptions.File.Interval, true, out RollingInterval interval))
        {
            interval = RollingInterval.Day;
        }

        loggerConfiguration.WriteTo.File(path, rollingInterval: interval,
            encoding: Encoding.UTF8, outputTemplate: logOptions.LogTemplate);
    }
}