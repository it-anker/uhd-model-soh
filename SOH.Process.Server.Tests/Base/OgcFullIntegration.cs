using Microsoft.AspNetCore.Hosting;

namespace SOH.Process.Server.Tests.Base;

public class OgcFullIntegration : AbstractOgcServices
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);
        builder.UseEnvironment("FullTest");
    }
}