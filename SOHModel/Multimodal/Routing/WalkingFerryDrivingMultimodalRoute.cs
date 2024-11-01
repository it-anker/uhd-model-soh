using Mars.Interfaces.Environments;
using SOHModel.Domain.Graph;
using SOHModel.Ferry.Station;

namespace SOHModel.Multimodal.Routing;

public class WalkingFerryDrivingMultimodalRoute : MultimodalRoute
{
    private readonly ISpatialGraphEnvironment _environment;
    private readonly IFerryStationLayer _ferryStationLayer;
    private readonly Position _goal;
    private readonly Position _start;

    /// <summary>
    ///     Describes a multimodal route with walk, ferry drive, walk.
    /// </summary>
    /// <param name="environmentLayer">Contains the environment.</param>
    /// <param name="stationLayer">The station layer containing all ferry stations to route to.</param>
    /// <param name="start">Position where the route should start.</param>
    /// <param name="goal">Position where the route should end.</param>
    public WalkingFerryDrivingMultimodalRoute(ISpatialGraphLayer environmentLayer,
        IFerryStationLayer stationLayer, Position start, Position goal)
    {
        _start = start;
        _goal = goal;

        _ferryStationLayer = stationLayer;
        _environment = environmentLayer.Environment;

        (FerryStation startFerryStation, Route? routeToFirstStation) = FindStartFerryStationAndWalkingRoute();
        (FerryStation goalFerryStation, Route? routeToGoal) = FindGoalFerryStationAndFinalWalkingRoute();
        IEnumerable<Route> ferryRoutes = FindFerryRoutes(startFerryStation, goalFerryStation);

        if (routeToFirstStation != null) Add(routeToFirstStation, ModalChoice.Walking);
        foreach (Route ferryRoute in ferryRoutes) Add(ferryRoute, ModalChoice.Ferry);
        if (routeToGoal != null) Add(routeToGoal, ModalChoice.Walking);
    }

    private (FerryStation, Route) FindStartFerryStationAndWalkingRoute(HashSet<FerryStation> unreachable = null)
    {
        unreachable ??= new HashSet<FerryStation>();
        FerryStation? ferryStation = _ferryStationLayer.Nearest(_start, station => !unreachable.Contains(station));
        if (ferryStation == null)
            throw new ApplicationException($"No reachable ferry station found for route from {_start} to {_goal}");

        ISpatialNode? startNode = _environment.NearestNode(_start, SpatialModalityType.Walking);
        ISpatialNode? ferryStationNode = _environment.NearestNode(ferryStation.Position, SpatialModalityType.Walking);
        if (startNode.Equals(ferryStationNode))
            return (ferryStation, null);

        Route? route = _environment.FindShortestRoute(startNode, ferryStationNode, WalkingFilter);
        if (route == null) // no walking route exists, ferry station is excluded from next search
        {
            unreachable.Add(ferryStation);
            return FindStartFerryStationAndWalkingRoute(unreachable);
        }

        // var distance = startNode.Position.DistanceInMTo(ferryStationNode.Position);
        // if (route.RouteLength > distance * 2)
        {
            FerryStation? nextFerryStation = _ferryStationLayer.Nearest(_start,
                station => !unreachable.Contains(station) && station != ferryStation);
            if (nextFerryStation != null)
            {
                ISpatialNode? nextFerryStationNode = _environment.NearestNode(nextFerryStation.Position);
                if (!startNode.Equals(nextFerryStationNode))
                {
                    Route? nextRoute = _environment.FindShortestRoute(startNode, nextFerryStationNode, WalkingFilter);
                    if (nextRoute?.RouteLength < route.RouteLength)
                        return (nextFerryStation, nextRoute);
                }
            }
        }

        return (ferryStation, route);
    }

    private (FerryStation, Route) FindGoalFerryStationAndFinalWalkingRoute(HashSet<FerryStation> unreachable = null)
    {
        unreachable ??= new HashSet<FerryStation>();
        FerryStation? ferryStation = _ferryStationLayer.Nearest(_goal, station => !unreachable.Contains(station));
        if (ferryStation == null)
            throw new ApplicationException(
                $"No ferry route available within the spatial graph environment to reach goal station from {_start} to {_goal}");

        ISpatialNode? ferryStationNode = _environment.NearestNode(ferryStation.Position, SpatialModalityType.Walking);
        ISpatialNode? goalNode = _environment.NearestNode(_goal, SpatialModalityType.Walking);
        if (ferryStationNode.Equals(goalNode))
            return (ferryStation, null);

        Route? route = _environment.FindShortestRoute(ferryStationNode, goalNode, WalkingFilter);
        if (route != null)
        {
            double distance = goalNode.Position.DistanceInMTo(ferryStationNode.Position);
            if (route.RouteLength > distance * 2)
            {
                FerryStation? nextFerryStation = _ferryStationLayer.Nearest(_goal,
                    station => !unreachable.Contains(station) && station != ferryStation);
                if (nextFerryStation != null)
                {
                    ISpatialNode? nextFerryStationNode = _environment.NearestNode(nextFerryStation.Position);
                    if (!goalNode.Equals(nextFerryStationNode))
                    {
                        Route? nextRoute =
                            _environment.FindShortestRoute(nextFerryStationNode, goalNode, WalkingFilter);
                        if (nextRoute?.RouteLength < route.RouteLength) return (nextFerryStation, nextRoute);
                    }
                }
            }

            return (ferryStation, route);
        }

        unreachable.Add(ferryStation);
        return FindGoalFerryStationAndFinalWalkingRoute(unreachable);
    }

    private IEnumerable<Route> FindFerryRoutes(FerryStation startFerryStation, FerryStation goalFerryStation)
    {
        List<Route> ferryRoutes = new List<Route>();
        ISpatialNode? startFerryStationNode =
            _environment.NearestNode(startFerryStation.Position, SpatialModalityType.ShipDriving);
        ISpatialNode? goalFerryStationNode =
            _environment.NearestNode(goalFerryStation.Position, SpatialModalityType.ShipDriving);

        if (startFerryStationNode.Equals(goalFerryStationNode)) return ferryRoutes;

        ISet<string> startLines = startFerryStation.Lines;
        ISet<string> goalLines = goalFerryStation.Lines;

        bool ShipDrivingFilter(ISpatialEdge edge)
        {
            return edge.Modalities.Contains(SpatialModalityType.ShipDriving);
        }

        if (startLines.Intersect(goalLines).Any()) // find direct line
        {
            Route? ferryRoute =
                _environment.FindShortestRoute(startFerryStationNode, goalFerryStationNode, ShipDrivingFilter);
            ferryRoutes.Add(ferryRoute);
        }
        else // find line with transfer point
        {
            FerryStation? transferPoint = _ferryStationLayer.Nearest(startFerryStation.Position,
                station => station.Lines.Intersect(startLines).Any() &&
                           station.Lines.Intersect(goalLines).Any());
            if (transferPoint != null) // single transfer point
            {
                ISpatialNode? transferFerryStationWaterwayNode =
                    _environment.NearestNode(transferPoint.Position, SpatialModalityType.ShipDriving);
                Route? ferryRoute1 = _environment.FindShortestRoute(startFerryStationNode,
                    transferFerryStationWaterwayNode, ShipDrivingFilter);
                ferryRoutes.Add(ferryRoute1);

                Route? ferryRoute2 = _environment.FindShortestRoute(transferFerryStationWaterwayNode,
                    goalFerryStationNode,
                    ShipDrivingFilter);
                ferryRoutes.Add(ferryRoute2);
            }
            else // multiple transfer points
            {
                IEnumerable<FerryStation> transferPointsStart = _ferryStationLayer.Features.OfType<FerryStation>().Where(
                    station => station != startFerryStation && station.Lines.Intersect(startLines).Any() &&
                               station.Lines.Count > 1);
                List<FerryStation> transferPointsGoal = _ferryStationLayer.Features.OfType<FerryStation>().Where(
                    station => station != goalFerryStation && station.Lines.Intersect(goalLines).Any() &&
                               station.Lines.Count > 1).ToList();

                foreach (FerryStation transferStart in transferPointsStart)
                foreach (FerryStation transferGoal in transferPointsGoal)
                    if (transferStart.Lines.Intersect(transferGoal.Lines).Any())
                    {
                        ISpatialNode? transferStartNode = _environment.NearestNode(transferStart.Position,
                            SpatialModalityType.ShipDriving);
                        ISpatialNode? transferGoalNode = _environment.NearestNode(transferGoal.Position,
                            SpatialModalityType.ShipDriving);

                        Route? ferryRoute1 = _environment.FindShortestRoute(startFerryStationNode, transferStartNode,
                            ShipDrivingFilter);
                        ferryRoutes.Add(ferryRoute1);

                        Route? ferryRoute2 = _environment.FindShortestRoute(transferStartNode, transferGoalNode,
                            ShipDrivingFilter);
                        ferryRoutes.Add(ferryRoute2);

                        Route? ferryRoute3 = _environment.FindShortestRoute(transferGoalNode, goalFerryStationNode,
                            ShipDrivingFilter);
                        ferryRoutes.Add(ferryRoute3);
                    }
            }
        }

        return ferryRoutes;
    }
}