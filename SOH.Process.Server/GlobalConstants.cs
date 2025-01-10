namespace SOH.Process.Server;

/// <summary>
///     Default keys and models, provided by this service.
/// </summary>
public static class GlobalConstants
{
    public const string FerryTransferId = $"sim-{FerryTransfer}-default";
    public const string FerryTransfer = "ferryTransfer";
    public const string FerryTransferDefaultConfig = "ferry_transfer_config.json";

    public const string Green4BikesId = $"sim-{Green4Bikes}-default";
    public const string Green4Bikes = "green4Bikes";
    public const string Green4BikesDefaultConfig = "green_4_bikes_config.json";

    public const int DefaultMaximumLengthDescription = 500;
    public const int DefaultMaximumLength = 50;
    public const int DefaultMaximumIdLength = 30;
    public const int DefaultStepRange = 1000;

    public static readonly HashSet<string> AvailableModelIds = [FerryTransferId, Green4BikesId];
}