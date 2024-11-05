namespace SOH.Process.Server.Middlewares;

internal static class Startup
{
    public static IServiceCollection AddMiddlewares(this IServiceCollection services, IConfiguration config)
    {
        return services
            .AddScoped<ExceptionMiddleware>()
            .AddSingleton<RequestLoggingMiddleware>()
            .AddScoped<ResponseLoggingMiddleware>();
    }

    public static IApplicationBuilder UseCustomMiddlewares(this IApplicationBuilder app)
    {
        return app.UseMiddleware<RequestLoggingMiddleware>()
            .UseMiddleware<ResponseLoggingMiddleware>();
    }

    public static IApplicationBuilder UseExceptionMiddleware(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ExceptionMiddleware>();
    }
}