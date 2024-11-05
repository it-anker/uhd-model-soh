namespace SOH.Process.Server;

public static class Program
{
    public interface IServerMarker;

    public static async Task Main(string[] args)
    {
        await Startup.RunOgcServiceAsync(args);
    }
}