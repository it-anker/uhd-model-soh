using Mars.Common;
using Mars.Components.Layers;
using Mars.Interfaces.Environments;

namespace SOHModel.Demonstration;

public class PoliceSchedulerLayer(DemonstrationLayer demonstrationLayer) : SchedulerLayer
{
    private DemonstrationLayer DemonstrationLayer { get; set; } = demonstrationLayer;

    protected override void Schedule(SchedulerEntry dataRow)
    {
        if (RegisterAgent == null) return;
        
        Position? source = dataRow.SourceGeometry.RandomPositionFromGeometry();
        Position? target = dataRow.TargetGeometry.RandomPositionFromGeometry();
        int squadSize = Convert.ToInt32(dataRow.Data["squadSize"]);

        Police police = new Police
        {
            Source = source,
            Target = target,
            SquadSize = squadSize
        };
        police.Init(DemonstrationLayer);
        DemonstrationLayer.PoliceMap[police.ID] = police;

        RegisterAgent(DemonstrationLayer, police);
    }
}