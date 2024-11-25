using System.Globalization;
using Microsoft.Extensions.DependencyInjection;

namespace SOH.Process.Server.Tests;

[Collection("Database collection")]
public abstract class AbstractManagementTests
{
    protected AbstractManagementTests(OgcIntegration services)
    {
        SmartOpenHamburg = services;
        var scope = services.Services.CreateScope();
        Services = scope.ServiceProvider;

        Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("de-DE");
        Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("de-DE");
    }

    protected OgcIntegration SmartOpenHamburg { get; }
    protected IServiceProvider Services { get; }
}