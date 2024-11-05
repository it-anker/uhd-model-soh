namespace SOH.Process.Server.Logging;

public class LogOptions
{
    public string Level { get; set; } = "information";
    public FileOptions? File { get; set; }
    public string LogTemplate { get; set; } = default!;
}