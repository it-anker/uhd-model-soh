namespace SOH.Process.Server.Persistence;

public class RedisDatabaseSettings
{
    public string ConnectionString { get; set; } = default!;

    public string DatabaseName { get; set; } = "ump_soh_simulations";

    public string? User { get; set; }

    public string? Password { get; set; }
}