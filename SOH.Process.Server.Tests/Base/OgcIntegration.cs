using Hangfire;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace SOH.Process.Server.Tests.Base;

public class OgcIntegration : AbstractOgcServices
{
    public Mock<IBackgroundJobClientV2> BackgroundMock { get; private set; } = new();
    public Mock<IRecurringJobManagerV2> RecurringJobMock { get; private set; } = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);

        builder.ConfigureServices(services =>
        {
            var dbContextDescriptor = services.SingleOrDefault(
                descriptor => descriptor.ServiceType == typeof(IBackgroundJobClientV2));
            if (dbContextDescriptor != null) services.Remove(dbContextDescriptor);
            BackgroundMock = new Mock<IBackgroundJobClientV2>();
            services.AddSingleton(typeof(IBackgroundJobClientV2), BackgroundMock.Object);

            var recurringJobDescriptor = services.SingleOrDefault(
                descriptor => descriptor.ServiceType == typeof(IRecurringJobManagerV2));
            if (recurringJobDescriptor != null) services.Remove(recurringJobDescriptor);
            RecurringJobMock = new Mock<IRecurringJobManagerV2>();
            services.AddSingleton(typeof(IRecurringJobManagerV2), RecurringJobMock.Object);
        });
    }
}