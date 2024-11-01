using System.Data;
using Mars.Common.Core;
using Mars.Components.Layers;

namespace SOHModel.Bus.Model;

public class BusSchedulerLayer : SchedulerLayer
{
    private readonly BusLayer _busLayer;

    public BusSchedulerLayer(BusLayer busLayer)
    {
        _busLayer = busLayer;
    }

    public BusSchedulerLayer(BusLayer busLayer, DataTable table) : base(table)
    {
        _busLayer = busLayer;
    }

    protected override void Schedule(SchedulerEntry dataRow)
    {
        const string typeKey = "busType";

        string? busType = dataRow.Data.TryGetValue(typeKey, out object? type) ? type.Value<string>() : "EvoBus";

        if (!dataRow.Data.ContainsKey("line"))
            throw new ArgumentException("Missing line number for bus of field 'line' in input");

        int boardingTime = dataRow.Data.TryGetValue("minimumBoardingTimeInSeconds", out object? wait)
            ? wait.Value<int>()
            : 0;
        bool reversedRoute = dataRow.Data.TryGetValue("reversedRoute", out object? reversed) &&
                             reversed.Value<bool>();
        BusDriver driver = new BusDriver(_busLayer, UnregisterAgent, busType)
        {
            Line = dataRow.Data["line"].Value<string>(),
            MinimumBoardingTimeInSeconds = boardingTime,
            ReversedRoute = reversedRoute
        };

        _busLayer.Driver.Add(driver.ID, driver);
        RegisterAgent(_busLayer, driver);
    }
}