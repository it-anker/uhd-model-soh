using Mars.Common;
using Mars.Components.Layers;
using Mars.Interfaces.Environments;

namespace SOHModel.Demonstration;

public class DemonstratorSchedulerLayer(DemonstrationLayer demonstrationLayer) : SchedulerLayer
{
    private DemonstrationLayer DemonstrationLayer { get; set; } = demonstrationLayer;

    protected override void Schedule(SchedulerEntry dataRow)
    {
        if (RegisterAgent == null) return;
        
        Position? source = dataRow.SourceGeometry.RandomPositionFromGeometry();
        Position? target = dataRow.TargetGeometry.RandomPositionFromGeometry();
        bool isRadical = Convert.ToBoolean(dataRow.Data["isRadical"]);

        if (isRadical)
        {
            RadicalDemonstrator demonstrator = new RadicalDemonstrator { Source = source, Target = target };
            demonstrator.Init(DemonstrationLayer);
            DemonstrationLayer.RadicalDemonstratorMap[demonstrator.ID] = demonstrator;
            RegisterAgent(DemonstrationLayer, demonstrator);
        }
        else
        {
            Demonstrator demonstrator = new Demonstrator { Source = source, Target = target };
            demonstrator.Init(DemonstrationLayer);
            DemonstrationLayer.DemonstratorMap[demonstrator.ID] = demonstrator;
            RegisterAgent(DemonstrationLayer, demonstrator);
        }
    }
}