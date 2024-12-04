using FluentValidation;
using IdempotentAPI.Cache.DistributedCache.Extensions.DependencyInjection;
using IdempotentAPI.Core;
using IdempotentAPI.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetTopologySuite.IO.Converters;
using Newtonsoft.Json;
using NJsonSchema;
using NSwag.Generation.Processors.Security;
using Serilog;
using SOH.Process.Server.Middlewares;
using SOH.Process.Server.Models;
using SOH.Process.Server.Simulations.Validators;

namespace SOH.Process.Server.Controllers;

public static class Startup
{
    private const string CorsPolicy = nameof(CorsPolicy);

    public static IServiceCollection AddApi(
        this IServiceCollection services, IConfiguration config)
    {
        ArgumentNullException.ThrowIfNull(config);

        services.AddHealthChecks();

        return services
            .Configure<OgcSettings>(config.GetSection(nameof(OgcSettings)))
            .AddLocalization()
            .AddResponseCompression(options => { options.EnableForHttps = true; })
            .AddDistributedMemoryCache()
            .AddMiddlewares(config)
            .AddCorsPolicy(config)
            .AddHttpClient()
            .AddHttpContextAccessor()
            .AddIdempotentAPIUsingDistributedCache()
            .AddRouting(options => options.LowercaseUrls = true)
            .AddValidatorsFromAssemblyContaining<ServerSimulationValidator>()
            .AddIdempotentAPI(new IdempotencyOptions
            {
                IsIdempotencyOptional = true, ExpireHours = 1,
                SerializerSettings = new JsonSerializerSettings
                {
                    DefaultValueHandling = DefaultValueHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    ObjectCreationHandling = ObjectCreationHandling.Replace,
                    DateFormatHandling = DateFormatHandling.IsoDateFormat,
                    Converters = [
                        new FeatureCollectionConverter(),
                        new FeatureConverter(),
                        new GeometryConverter()
                    ]
                }
            })
            .AddControllers(options =>
            {
                options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
            })
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                options.SerializerSettings.DefaultValueHandling = DefaultValueHandling.Include;
                options.SerializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
                options.SerializerSettings.Converters.Add(new FeatureCollectionConverter());
                options.SerializerSettings.Converters.Add(new FeatureConverter());
                options.SerializerSettings.Converters.Add(new GeometryConverter());
                options.SerializerSettings.Converters.Add(new AttributesTableConverter());

            })
            .Services
            .AddSwaggerGen()
            .AddEndpointsApiExplorer()
            .AddOpenApiDocument(document =>
            {
                document.DocumentName = "ogc-processes";
                document.Description = "Official OGC Processes API Endpoint for Simulation Execution";
                document.ApiGroupNames = ["ogc-processes"];
                document.Version = "0.1.0";
                document.SchemaSettings.SchemaType = SchemaType.OpenApi3;
                document.UseControllerSummaryAsTagDescription = true;
                document.SchemaSettings.AlwaysAllowAdditionalObjectProperties = true;
                document.SchemaSettings.AllowReferencesWithProperties = true;
                document.PostProcess = doc =>
                {
                    doc.Info.Title = "OGC Process API Simulation";
                    doc.Info.Version = "0.1.0";
                };

                document.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor());
            });
    }

    public static IApplicationBuilder UseControllers(this IApplicationBuilder builder, IWebHostEnvironment environment)
    {
        builder
            .UseSerilogRequestLogging()
            .UseExceptionMiddleware()
            .UseHsts()
            .UseRouting()
            .UseCorsPolicy()
            .UseOpenApi()
            .UseSwagger()
            .UseSwaggerUi(options =>
            {
                options.DefaultModelsExpandDepth = -1;
                options.DocExpansion = "none";
                options.TagsSorter = "alpha";
                options.ValidateSpecification = true;
                options.DocumentTitle = "Urban Model Plattform - SmartOpenHamburg OGC Processes API";
                options.CustomHeadContent = "Swagger Documentation of the SmartOpenHamburg OGC Processes API Endpoint" +
                                            " for Simulation- and Execution Management";
            })
            .UseCustomMiddlewares()
            .UseSwagger()
            .UseResponseCompression()
            .UseEndpoints(routeBuilder => routeBuilder.MapControllerEndpoints());

        if (environment.IsProduction())
        {
            builder.UseHealthChecks("/healthz",
                new HealthCheckOptions
                {
                    Predicate = _ => true,
                    ResultStatusCodes =
                    {
                        [HealthStatus.Healthy] = StatusCodes.Status200OK,
                        [HealthStatus.Degraded] = StatusCodes.Status500InternalServerError,
                        [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
                    }
                });
        }

        return builder;
    }

    private static IEndpointRouteBuilder MapControllerEndpoints(this IEndpointRouteBuilder builder)
    {
        builder
            .MapControllers()
            .RequireCors();
        builder
            .MapHealthChecks("/api/health");
        return builder;
    }

    private static IServiceCollection AddCorsPolicy(this IServiceCollection services, IConfiguration config)
    {
        var corsSettings = config.GetSection(nameof(CorsSettings)).Get<CorsSettings>();
        var origins = new List<string>();
        if (corsSettings is { Origins: not null })
        {
            string[] corsPaths = corsSettings.Origins.Split(';', StringSplitOptions.RemoveEmptyEntries);
            origins.AddRange(corsPaths);
        }

        services.AddCors(opt =>
        {
            opt.AddPolicy(CorsPolicy, policy =>
            {
                policy
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .SetIsOriginAllowed(_ => true)
                    .AllowCredentials()
                    .WithOrigins(origins.ToArray());
            });
        });

        return services;
    }

    private static IApplicationBuilder UseCorsPolicy(this IApplicationBuilder app)
    {
        return app.UseCors(CorsPolicy);
    }
}

public class CorsSettings
{
    public string? Origins { get; set; }
}