using Microsoft.Extensions.DependencyInjection;

namespace SOH.Process.Server.Tests.Base;

[CollectionDefinition("Full collection")]
public abstract class AbstractFullManagementTests
{
    protected AbstractFullManagementTests(OgcFullIntegration services)
    {
        SmartOpenHamburg = services;
        var scope = services.Services.CreateScope();
        Services = scope.ServiceProvider;
    }

    protected OgcFullIntegration SmartOpenHamburg { get; }
    protected IServiceProvider Services { get; }
}