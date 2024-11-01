using Mars.Interfaces.Environments;
using SOHModel.Bicycle.Rental;
using SOHModel.Domain.Graph;

namespace SOHModel.Multimodal.Routing;

internal class WalkingCyclingMultimodalRoute : MultimodalRoute
{
    /// <summary>
    ///     Describes a multimodal route with walk, cycle, walk.
    /// </summary>
    /// <param name="envLayer">Contains the environment.</param>
    /// <param name="bicycleRentalLayer">Provides the possibility to find rental stations.</param>
    /// <param name="start">Position where the route should start.</param>
    /// <param name="goal">Position where the route should end.</param>
    public WalkingCyclingMultimodalRoute(
        ISpatialGraphLayer envLayer, IBicycleRentalLayer bicycleRentalLayer,
        Position start, Position goal)
    {
        ISpatialGraphEnvironment env = envLayer.Environment;

        ISpatialNode? startNode = env.NearestNode(start, SpatialModalityType.Walking);
        BicycleRentalStation? startRental = bicycleRentalLayer.Nearest(start, true);
        if (startRental == null)
            throw new ArgumentException(
                "Could not find any route for bicycle because no rental bicycle is available.");

        ISpatialNode? startRentalSidewalkNode = env.NearestNode(startRental.Position, SpatialModalityType.Walking);

        if (!startNode.Equals(startRentalSidewalkNode))
        {
            Route? routeToRentalStart = env.FindShortestRoute(startNode, 
                startRentalSidewalkNode, WalkingFilter);
            if (routeToRentalStart != null)
                Add(routeToRentalStart, ModalChoice.Walking);
        }


        ISpatialNode? startRentalStreetNode = env.NearestNode(startRentalSidewalkNode.Position, SpatialModalityType.Cycling);
        BicycleRentalStation? goalRental = bicycleRentalLayer.Nearest(goal, false);
        
        ISpatialNode? goalRentalStreetNode = env.NearestNode(goalRental.Position, SpatialModalityType.Cycling);

        if (startRentalStreetNode.Equals(goalRentalStreetNode))
            throw new ArgumentException($"Could not find any route for bicycle from {start} to {goal}");

        Route cyclingRoute = FindCyclingRoute(env, startRentalStreetNode, goalRentalStreetNode);
        foreach (EdgeStop? stop in cyclingRoute)
            stop.DesiredLane = 0; // TODO necessary because of bug in bicycle steering
        Add(cyclingRoute, ModalChoice.CyclingRentalBike);

        ISpatialNode? goalRentalSidewalkNode = env.NearestNode(goalRentalStreetNode.Position, SpatialModalityType.Walking);
        ISpatialNode? goalNode = env.NearestNode(goal, SpatialModalityType.Walking);

        if (!goalRentalSidewalkNode.Equals(goalNode))
        {
            Route? routeToGoal = env.FindShortestRoute(goalRentalSidewalkNode, goalNode, WalkingFilter);
            if (routeToGoal != null)
                Add(routeToGoal, ModalChoice.Walking);
        }
    }

    /// <summary>
    ///     Describes a multimodal route with walk, cycle with own bike, walk.
    /// </summary>
    /// <param name="envLayer">Contains the environment.</param>
    /// <param name="bicycle">The bicycle is located at a certain position.</param>
    /// <param name="start">Position where the route should start.</param>
    /// <param name="goal">Position where the route should end.</param>
    public WalkingCyclingMultimodalRoute(
        ISpatialGraphLayer envLayer, IPositionable bicycle, Position start, Position goal)
    {
        ISpatialGraphEnvironment env = envLayer.Environment;
        ISpatialNode? startNode = env.NearestNode(start, SpatialModalityType.Walking);
        ISpatialNode? bicycleNode = env.NearestNode(bicycle.Position, SpatialModalityType.Walking);

        if (!startNode.Equals(bicycleNode))
        {
            Route? routeToRentalStart = env.FindShortestRoute(startNode, bicycleNode,
                edge => edge.Modalities.Contains(SpatialModalityType.Walking));
            Add(routeToRentalStart, ModalChoice.Walking);
        }

        ISpatialNode? startStreetNode = env.NearestNode(bicycle.Position, SpatialModalityType.Cycling);
        ISpatialNode? goalStreetNode = env.NearestNode(goal, SpatialModalityType.Cycling);

        if (!startStreetNode.Equals(goalStreetNode))
        {
            Route cyclingRoute = FindCyclingRoute(env, startStreetNode, goalStreetNode);
            foreach (EdgeStop? stop in cyclingRoute)
                stop.DesiredLane = 0; // TODO necessary because of bug in bicycle steering
            Add(cyclingRoute, ModalChoice.CyclingOwnBike);
        }

        ISpatialNode? goalCyclingNode = env.NearestNode(goalStreetNode.Position, SpatialModalityType.Walking);
        ISpatialNode? goalWalkingNode = env.NearestNode(goal, SpatialModalityType.Walking);

        if (!goalCyclingNode.Equals(goalWalkingNode))
        {
            Route? routeToGoal = env.FindShortestRoute(goalCyclingNode, goalWalkingNode, WalkingFilter);
            Add(routeToGoal, ModalChoice.Walking);
        }
    }

    private static Route FindCyclingRoute(IRoutePlanner environment, ISpatialNode startNode,
        ISpatialNode goalNode)
    {
        Route? route = environment.FindShortestRoute(startNode, goalNode,
            edge => edge.Modalities.Contains(SpatialModalityType.Cycling) ||
                    edge.Modalities.Contains(SpatialModalityType.CarDriving));
        if (route != null) return route;

        // const int hops = 3;
        // foreach (var tempStartNode in environment.NearestNodes(startNode.Position, double.MaxValue, hops))
        // foreach (var tempGoalNode in environment.NearestNodes(goalNode.Position, double.MaxValue, hops))
        // {
        //     route = environment.FindShortestRoute(tempStartNode, tempGoalNode);
        //     if (route != null) return route;
        // }

        throw new ArgumentException(
            $"Could not find any route for bicycle from {startNode.Position} to {goalNode.Position}");
    }
}