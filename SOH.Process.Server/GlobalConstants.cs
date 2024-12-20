namespace SOH.Process.Server;

/// <summary>
///     Default keys and models, provided by this service.
/// </summary>
public static class GlobalConstants
{
    public const string FerryTransferId = $"simulation:{FerryTransfer}:fc1e588a-1595-42a3-bd58-eba1382f54c0";
    public const string FerryTransfer = "ferryTransfer";
    public const string FerryTransferDefaultConfig = "ferry_transfer_config.json";

    public const string Green4BikesId = $"simulation:{Green4Bikes}:e475eb85-673e-4541-bded-3c809f458454";
    public const string Green4Bikes = "green4Bikes";
    public const string Green4BikesDefaultConfig = "green_4_bikes_config.json";


    public const int DefaultStepRange = 1000;

    public static readonly HashSet<string> AvailableModelIds = [FerryTransferId, Green4BikesId];
}