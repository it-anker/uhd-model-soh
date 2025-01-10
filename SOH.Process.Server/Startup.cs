using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.HttpOverrides;
using Serilog;
using SOH.Process.Server.Controllers;
using SOH.Process.Server.Middlewares;
using SOH.Process.Server.Persistence;
using SOH.Process.Server.Simulations;
using SOH.Process.Server.Simulations.Services;
using SOH.Process.Server.Simulations.Workflows;
using SOH.Process.Server.Validation;

namespace SOH.Process.Server;

public static class Startup
{
    public static async Task RunOgcServiceAsync(string[] args)
    {
        Log.Information("OGC Processes API server is starting ...");
        var builder = CreateWebApplicationBuilder(args);

        await using var app = builder
            .BuildApplication()
            .Build();

        await app.ConfigureRuntimeServer();
        await app.RunAsync();
        Log.Information("Server Shutting down...");
    }

    private static WebApplicationBuilder CreateWebApplicationBuilder(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Host.UseSerilog((hostingContext, loggerConfiguration) => loggerConfiguration
            .ReadFrom.Configuration(hostingContext.Configuration));

        var executionEnvironment = builder.Environment;
        string environmentName = string.IsNullOrEmpty(executionEnvironment.EnvironmentName)
            ? "Development"
            : executionEnvironment.EnvironmentName;
        string applicationName = string.IsNullOrEmpty(executionEnvironment.ApplicationName)
            ? "OGC Process API"
            : executionEnvironment.ApplicationName;

        builder.Configuration
            .AddJsonFile("appsettings.json", false, true)
            .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json",
                true, true)
            .AddEnvironmentVariables();

        builder
            .WebHost
            .UseQuic()
            .UseKestrel(options => options.AddServerHeader = false)
            .ConfigureKestrel(options =>
            {
                options.Limits.MaxConcurrentConnections = 1000;
                options.Limits.MaxRequestBodySize = 100_000_000;
            })
            .UseShutdownTimeout(TimeSpan.FromSeconds(3));

        Log.Information(
            "Start {ApplicationName} in {EnvironmentName} mode",
            applicationName,
            environmentName);
        return builder;
    }

    private static WebApplicationBuilder BuildApplication(this WebApplicationBuilder builder)
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();

        builder.Services
            .AddScoped<ISimulationService, SimulationServiceImpl>()
            .AddScoped<IResultService, ResultsServiceImpl>()
            .AddScoped<ICustomSeeder, ModelSeeder>()
            .AddScoped<JobSubscribeWorkflow>()
            .AddScoped<SimulationResultWorkflow>()
            .AddValidatorsFromAssemblies(assemblies)
            .AddMediatR(config =>
                config.RegisterServicesFromAssemblies(assemblies)
                    .AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>))
                    .AddBehavior(typeof(IPipelineBehavior<,>), typeof(PrepareRequestBehavior<,>))
                    .AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>)))
            .AddPersistence(builder.Configuration, builder.Environment)
            .AddApi(builder.Configuration);

        Log.Information("Process hosting: {ProcessName}",
            System.Diagnostics.Process.GetCurrentProcess().ProcessName);

        return builder;
    }

    private static Task ConfigureRuntimeServer(this WebApplication app)
    {
        Log.Information("Start system in {Development} mode...",
            app.Environment.EnvironmentName.ToLower());

        if (!app.Environment.IsDevelopment())
        {
            // The default HSTS value is 30 days. Maybe we change this
            // for other scenarios see https://aka.ms/aspnetcore-hsts.
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor |
                                       ForwardedHeaders.XForwardedProto |
                                       ForwardedHeaders.XForwardedHost
            })
                .UseExceptionHandler("/Error")
                .UseHttpsRedirection()
                .UseHsts();
        }

        app
            .UseProcesses(app.Environment, app.Configuration)
            .UseControllers(app.Environment);

        Log.Information("OGC Processes API server is running");
        return Task.CompletedTask;
    }
}