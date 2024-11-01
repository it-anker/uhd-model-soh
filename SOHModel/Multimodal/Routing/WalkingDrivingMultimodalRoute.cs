using Mars.Interfaces.Environments;
using SOHModel.Car.Parking;
using SOHModel.Domain.Graph;

namespace SOHModel.Multimodal.Routing;

public class WalkingDrivingMultimodalRoute : MultimodalRoute
{
    private static readonly Func<ISpatialEdge, bool> CarDrivingFilter =
        edge => edge.Modalities.Contains(SpatialModalityType.CarDriving);

    public WalkingDrivingMultimodalRoute(ISpatialGraphLayer envLayer, Car.Model.Car car, Position start, Position goal)
    {
        ISpatialGraphEnvironment env = envLayer.Environment;

        ISpatialNode? startAgentNode = env.NearestNode(start, SpatialModalityType.Walking);
        ISpatialNode? startParkingNode = env.NearestNode(car.Position, SpatialModalityType.Walking);

        if (!startAgentNode.Equals(startParkingNode))
        {
            Route? routeToCar = env.FindShortestRoute(startAgentNode, startParkingNode, WalkingFilter);
            Add(routeToCar, ModalChoice.Walking);
        }

        CarParkingSpace? goalParkingSpace = car.CarParkingLayer.Nearest(goal);
        if (goalParkingSpace == null)
            throw new ArgumentException($"Could not find any free parking space near {goal}");

        ISpatialNode? startParkingStreetNode = env.NearestNode(startParkingNode.Position, SpatialModalityType.Walking,
            SpatialModalityType.CarDriving);
        ISpatialNode? goalParkingStreetNode = env.NearestNode(goalParkingSpace.Position, SpatialModalityType.CarDriving,
            SpatialModalityType.Walking);
        if (startParkingStreetNode.Equals(goalParkingStreetNode))
            throw new ArgumentException($"Start and goal node are the same: {goal}");

        Route routeWithCar = FindDrivingRoute(env, startParkingStreetNode, goalParkingStreetNode);
        Add(routeWithCar, ModalChoice.CarDriving);

        ISpatialNode? goalParkingSidewalkNode = env.NearestNode(goalParkingStreetNode.Position, SpatialModalityType.Walking);
        ISpatialNode? goalAgentNode = env.NearestNode(goal, SpatialModalityType.Walking);
        if (!goalAgentNode.Equals(goalParkingSidewalkNode))
        {
            Route? routeToGoal = env.FindShortestRoute(goalParkingSidewalkNode, goalAgentNode, WalkingFilter);
            Add(routeToGoal, ModalChoice.Walking);
        }
    }

    /// <summary>
    ///     Driver is currently driving, so only a parking has to be searched and a remaining walk to goal.
    /// </summary>
    /// <param name="environmentLayer">Provides access to spatial structures.</param>
    /// <param name="car">That is used.</param>
    /// <param name="goal">To reach.</param>
    public WalkingDrivingMultimodalRoute(
        ISpatialGraphLayer environmentLayer, Car.Model.Car car, Position goal)
    {
        ISpatialGraphEnvironment street = environmentLayer.Environment;
        CarParkingSpace? parkingSpace = car.CarParkingLayer.Nearest(goal);
        if (parkingSpace == null)
            throw new ArgumentException("Could not reroute because no free parking space could be found.");

        ISpatialNode? currentNode = car.CurrentEdge.To;
        ISpatialNode? nodeParkingGoalStreet = street.NearestNode(parkingSpace.Position, SpatialModalityType.CarDriving);

        ISpatialGraphEnvironment sidewalk = environmentLayer.Environment;
        if (!currentNode.Equals(nodeParkingGoalStreet))
        {
            Route routeWithCar = FindDrivingRoute(street, currentNode, nodeParkingGoalStreet);
            Add(routeWithCar, ModalChoice.CarDriving);
        }

        ISpatialNode? nodeParkingGoalSidewalk =
            sidewalk.NearestNode(nodeParkingGoalStreet.Position, SpatialModalityType.Walking);
        ISpatialNode? nodeGoal = sidewalk.NearestNode(goal, SpatialModalityType.Walking);
        if (!nodeGoal.Equals(nodeParkingGoalSidewalk))
        {
            Route? routeToGoal = sidewalk.FindShortestRoute(nodeParkingGoalSidewalk, nodeGoal, WalkingFilter);
            Add(routeToGoal, ModalChoice.Walking);
        }
    }

    private static Route FindDrivingRoute(ISpatialGraphEnvironment environment, ISpatialNode startNode,
        ISpatialNode goalNode)
    {
        Route? route = environment.FindFastestRoute(startNode, goalNode, CarDrivingFilter);
        if (route != null) return route;

        const int hops = 3;
        foreach (ISpatialNode? tempStartNode in environment.NearestNodes(startNode.Position, double.MaxValue, hops))
        foreach (ISpatialNode? tempGoalNode in environment.NearestNodes(goalNode.Position, double.MaxValue, hops))
        {
            route = environment.FindFastestRoute(tempStartNode, tempGoalNode, CarDrivingFilter);
            if (route != null) return route;
        }

        throw new ArgumentException(
            $"Could not find any route for car from {startNode.Position} to {goalNode.Position}");
    }
}