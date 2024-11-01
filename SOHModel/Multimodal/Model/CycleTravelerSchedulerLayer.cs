using Mars.Common;
using Mars.Common.Core;
using Mars.Components.Layers;
using Mars.Interfaces.Environments;

namespace SOHModel.Multimodal.Model;

/// <summary>
///     Provides the scheduling of <see cref="CycleTraveler" /> utilizing the cycle and the walking modality.
/// </summary>
public class CycleTravelerSchedulerLayer : SchedulerLayer
{
    private readonly Random _random;
    private readonly CycleTravelerLayer _travelerLayer;

    public CycleTravelerSchedulerLayer(CycleTravelerLayer travelerLayer)
    {
        _travelerLayer = travelerLayer;
        _random = new Random();
    }

    protected override void Schedule(SchedulerEntry dataRow)
    {
        Position? source = dataRow.SourceGeometry.RandomPositionFromGeometry();
        Position? target = dataRow.TargetGeometry.RandomPositionFromGeometry();

        double ownBike = dataRow.Data["hasOwnBikeProbability"].Value<double>();
        bool hasBike = _random.NextDouble() < ownBike;

        CycleTraveler traveler = new CycleTraveler
        {
            HasBike = hasBike, StartPosition = source, GoalPosition = target
        };
        traveler.Init(_travelerLayer);

        RegisterAgent(_travelerLayer, traveler);
    }
}