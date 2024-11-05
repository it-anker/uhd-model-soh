using Hangfire;
using Newtonsoft.Json;

namespace SOH.Process.Server.Background;

public static class HangfireConfigurationExtensions
{
    public static void UseMediatR(this IGlobalConfiguration configuration)
    {
        var jsonSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All
        };
        configuration.UseSerializerSettings(jsonSettings);
    }
}