namespace SOH.Process.Server.Simulations;

public static class Startup
{
    public static IApplicationBuilder UseProcesses(
        this IApplicationBuilder builder, IHostEnvironment env, IConfiguration config)
    {
        SeedDataAsync(builder.ApplicationServices)
            .GetAwaiter().GetResult();

        return builder;
    }

    private static async Task SeedDataAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();

        var seeders = scope.ServiceProvider.GetServices<ICustomSeeder>().ToArray();
        foreach (var seeder in seeders)
        {
            await seeder.InitializeAsync(CancellationToken.None);
        }
    }
}