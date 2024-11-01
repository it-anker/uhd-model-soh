using Mars.Interfaces.Environments;
using SOHModel.Multimodal.Model;

namespace SOHModel.Demonstration;

public class PoliceChief : MultiCapableAgent<DemonstrationLayer>
{
    private DemonstrationLayer? _demonstrationLayer;

    public override void Init(DemonstrationLayer layer)
    {
        base.Init(layer);
        _demonstrationLayer = layer;

        EnvironmentLayer = _demonstrationLayer.SpatialGraphMediatorLayer;

        // StartPosition = new Position(9.955743, 53.570198);
        // GoalPosition = new Position(9.952852, 53.545340);

        // Get roadblock positions on each side of the demonstration route
        List<ISpatialNode> leftNodes = _demonstrationLayer.LeftPoliceRouteNodes.ToList();
        List<ISpatialNode> rightNodes = _demonstrationLayer.RightPoliceRouteNodes.ToList();
        
        // Calculate some counts for later calculations
        int leftNodeCount = leftNodes.Count;
        int rightNodeCount = rightNodes.Count;
        int nodeCount = leftNodeCount + rightNodeCount;
        int policeCount = _demonstrationLayer.PoliceMap.Count;
        
        // Calculate how many Police agents can be positioned on each side of the demonstration route
        double leftPoliceRatio = (double)leftNodeCount / nodeCount;
        int leftPoliceCount = (int)Math.Round(policeCount * leftPoliceRatio);
        int rightPoliceCount = policeCount - leftPoliceCount;

        // Calculate the spacing of the available Police agents on each side of the demonstration route
        // Goal: distribute police units as evenly along each side of the demonstration route as possible
        double leftPoliceDist = Math.Pow((double)leftPoliceCount / leftNodeCount, -1);
        double rightPoliceDist = Math.Pow((double)rightPoliceCount / rightNodeCount, -1);

        //0.5 -> 2
        //2 -> 0.5
        // Get an enumerator to iterate over Police agents
        using IEnumerator<Police> policeEnum = _demonstrationLayer.PoliceMap.Values.GetEnumerator();
        
        // Distribute Police agents on left side of demonstration route
        for (double i = 0; i < _demonstrationLayer.LeftPoliceRouteNodes.Count; i += leftPoliceDist)
        {
            int index = (int)Math.Floor(i);
            if (policeEnum.MoveNext())
            {
                policeEnum.Current.Source = leftNodes[index].Position;
                policeEnum.Current.Position = policeEnum.Current.Source;
            }
        }

        // Distribute Police agents on right side of demonstration route
        for (double i = 0; i < _demonstrationLayer.RightPoliceRouteNodes.Count; i += rightPoliceDist)
        {
            int index = (int)Math.Floor(i);
            if (policeEnum.MoveNext())
            {
                policeEnum.Current.Source = rightNodes[index].Position;
                policeEnum.Current.Position = policeEnum.Current.Source;
            }
        }
    }

    public override void Tick()
    {
        // do nothing
    }
}