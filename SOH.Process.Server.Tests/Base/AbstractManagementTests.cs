using Microsoft.Extensions.DependencyInjection;

namespace SOH.Process.Server.Tests.Base;

[Collection("Database collection")]
public abstract class AbstractManagementTests
{
    protected AbstractManagementTests(OgcIntegration services)
    {
        SmartOpenHamburg = services;
        var scope = services.Services.CreateScope();
        Services = scope.ServiceProvider;
    }

    protected OgcIntegration SmartOpenHamburg { get; }
    protected IServiceProvider Services { get; }
}