namespace SOH.Process.Server.Simulations;

public interface ICustomSeeder
{
    Task InitializeAsync(CancellationToken cancellationToken);
}