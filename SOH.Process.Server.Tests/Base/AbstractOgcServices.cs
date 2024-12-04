using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Testcontainers.Redis;

namespace SOH.Process.Server.Tests.Base;

/// <summary>
///     Use this base class, and inherit from it to construct a bootstrap web application, to testing
///     against the internal and external interfaces with all your required services.
/// </summary>
/// <remarks>
///     This fixture and application cleans up all data (except master-data) when starting (only at the beginning
///     of the complete test process) and after the last test case has been executed.
/// </remarks>
public abstract class AbstractOgcServices : WebApplicationFactory<Program.IServerMarker>, IAsyncLifetime
{
    private readonly RedisContainer _redisContainer = new RedisBuilder()
        .WithImage("redis:7.4.1-alpine3.20")
        .Build();

    public HttpClient RootUserWithToken { get; private set; } = default!;

    public virtual async Task InitializeAsync()
    {
        await InitContainers();
        await SetClients();
    }

    private async Task InitContainers()
    {
        await _redisContainer.StartAsync();
    }

    Task IAsyncLifetime.DisposeAsync()
    {
        return Task.CompletedTask;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        string? redisConnectionString = _redisContainer.GetConnectionString();

        builder.UseEnvironment("Test");
        builder.UseSetting("Redis:ConnectionString", redisConnectionString);
        builder.UseSetting("Redis:UseTestcontainers", false.ToString());
    }

    private Task SetClients()
    {
        RootUserWithToken = CreateClient();
        return Task.CompletedTask;
    }
}