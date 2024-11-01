using Mars.Common;
using Mars.Components.Environments;
using Mars.Interfaces.Agents;
using Mars.Interfaces.Environments;
using SOHModel.Domain.Model;
using SOHModel.Domain.Steering.Acceleration;
using SOHModel.Domain.Steering.Capables;
using SOHModel.Domain.Steering.Handles.Intersection;

namespace SOHModel.Domain.Steering.Handles;

public class VehicleSteeringHandle
    <TSteeringCapable, TPassengerCapable, TSteeringHandle, TPassengerHandle> :
        VehiclePassengerHandle<TSteeringCapable, TPassengerCapable, TSteeringHandle, TPassengerHandle>,
        ISteeringHandle
    where TSteeringCapable : ISteeringCapable
    where TPassengerCapable : IPassengerCapable
    where TSteeringHandle : ISteeringHandle
    where TPassengerHandle : IPassengerHandle
{
    protected const double MaximalDeceleration = 1000;
    private const int IntersectionAheadClearanceInM = 150;
    private const int FreeDrivingClearanceInM = 1000;
    protected const double UrbanSafetyDistanceInM = 50d;
    private const double FiftyKmHinMs = 50d / 3.6;
    private const int MinimalExploreDistance = 30;
    private const double SafetyDistanceForOvertaking = 20; // TODO literature?

    private IIntersectionTrafficCode _intersectionTrafficCode;
    protected IVehicleAccelerator VehicleAccelerator;

    public VehicleSteeringHandle(ISpatialGraphEnvironment environment,
        Vehicle<TSteeringCapable, TPassengerCapable, TSteeringHandle, TPassengerHandle> vehicle,
        double standardSpeedLimit = FiftyKmHinMs) : base(vehicle)
    {
        Environment = environment;
        Route = new Route();
        StandardSpeedLimit = standardSpeedLimit;
        VehicleAccelerator = new IntelligentDriverAccelerator();
        SetIntersectionTrafficCode(vehicle.TrafficCode);
        NextTrafficLightPhase = TrafficLightPhase.None;
    }

    public double RemainingDistanceOnEdge => Vehicle.RemainingDistanceOnEdge;

    public double SpeedLimit => Vehicle.CurrentEdge?.MaxSpeed ?? StandardSpeedLimit;
    private double StandardSpeedLimit { get; }

    private double MaxSpeed => Math.Min(SpeedLimit, Vehicle.MaxSpeed);

    public TrafficLightPhase NextTrafficLightPhase { get; protected set; }

    public Route Route { get; set; }

    public ISpatialGraphEnvironment Environment { get; }
    public double Velocity => Vehicle.Velocity;
    public bool GoalReached => Route?.GoalReached ?? true;

    public virtual void Move()
    {
        if (GoalReached || (Vehicle.CurrentEdge == null && !MoveFromNodeSuccessfully())) return;

        // TODO stay on lane when going on next edge?? going right left, before turning on intersection?

        SpatialGraphExploreResult exploreResult = ExploreEnvironment();
        double deceleration = MaximalDeceleration;
        deceleration = HandleBraking(deceleration);
        deceleration = HandleIntersectionAhead(exploreResult, deceleration);
        deceleration = HandleVehiclesAhead(exploreResult, deceleration);

        double drivingDistance = CalculateDrivingDistance(deceleration);
        PerformMoveAction(drivingDistance);
    }

    private double HandleBraking(double deceleration)
    {
        if (!Vehicle.Driver.BrakingActivated)
            return deceleration;

        double distance = Math.Pow(Velocity * 3.6 / 10, 2) * 2;
        return CalculateSpeedChange(Velocity, MaxSpeed, distance, 0, 0);
    }

    public void SetIntersectionTrafficCode(string trafficCode)
    {
        _intersectionTrafficCode = trafficCode switch
        {
            "south-african" => new FifoIntersectionHandle
                <TSteeringCapable, TPassengerCapable, TSteeringHandle, TPassengerHandle>(Vehicle,
                    VehicleAccelerator),
            _ => new RightBeforeLeftIntersectionHandle<TSteeringCapable, TPassengerCapable, TSteeringHandle,
                TPassengerHandle>(Vehicle, VehicleAccelerator)
        };
    }

    protected SpatialGraphExploreResult ExploreEnvironment()
    {
        return Environment.Explore(Vehicle, Route,
            Math.Max(MinimalExploreDistance, Vehicle.ExploreDistanceFactor * Vehicle.Velocity));
    }

    protected virtual double HandleIntersectionAhead(SpatialGraphExploreResult exploreResult,
        double biggestDeceleration)
    {
        NextTrafficLightPhase = exploreResult.EdgeExplores.FirstOrDefault()?.LightPhase ?? TrafficLightPhase.None;

        if (Vehicle.RemainingDistanceOnEdge > IntersectionAheadClearanceInM)
            return biggestDeceleration;

        List<EdgeExploreResult>? exploreResults = exploreResult.EdgeExplores;
        bool approachingEndOfRoute =
            Route.RemainingRouteDistanceToGoal < UrbanSafetyDistanceInM && exploreResults.Count == 1;
        if (approachingEndOfRoute)
        {
            double speedChange = CalculateSpeedChange(Vehicle.Velocity, MaxSpeed,
                exploreResults.First().IntersectionDistance, 0, 0);
            return Math.Min(speedChange, biggestDeceleration);
        }

        for (int index = 0; index < exploreResults.Count - 1; index++)
        {
            EdgeExploreResult edgeExploreResult = exploreResults[index];
            //-------------------------------//
            //** Turning Speed Adjustment ***//
            //-------------------------------//
            DirectionType nextDirection = DirectionType.Up;

            if (edgeExploreResult.IntersectionDistance < UrbanSafetyDistanceInM)
            {
                nextDirection = edgeExploreResult.Edge.To.GetDirection(edgeExploreResult.Edge,
                    exploreResults[index + 1].Edge);
                double turningSpeed = Vehicle.TurningSpeedFor(nextDirection);

                if (turningSpeed > 0)
                {
                    if (Vehicle.Velocity > turningSpeed)
                    {
                        double speedChange = CalculateSpeedChange(Vehicle.Velocity, MaxSpeed,
                            edgeExploreResult.IntersectionDistance, turningSpeed, 0);

                        // TODO have to check this on semantics
                        if (Vehicle.Velocity + speedChange < turningSpeed)
                        {
                            double a = turningSpeed - Vehicle.Velocity;
                            if (a < biggestDeceleration)
                                biggestDeceleration = a;
                            continue;
                        }

                        if (speedChange < biggestDeceleration)
                            biggestDeceleration = speedChange;
                    }
                    else
                    {
                        double speedChange = CalculateSpeedChange(Vehicle.Velocity,
                            turningSpeed, FreeDrivingClearanceInM, turningSpeed, 0);

                        if (speedChange < biggestDeceleration)
                            biggestDeceleration = speedChange;
                    }
                }
            }

            //-------------------------------//
            //** Traffic Lights Adjustments *//
            //-------------------------------//
            if (edgeExploreResult.LightPhase == TrafficLightPhase.None &&
                edgeExploreResult.IntersectionDistance < UrbanSafetyDistanceInM)
            {
                bool intersectionWithMultipleEdges = edgeExploreResult.Edge.To.IncomingEdges.Count > 1;
                if (intersectionWithMultipleEdges)
                {
                    double deceleration = _intersectionTrafficCode.Evaluate(edgeExploreResult, nextDirection);
                    if (deceleration < biggestDeceleration)
                        biggestDeceleration = deceleration;
                }
            }
            else if (edgeExploreResult.LightPhase == TrafficLightPhase.Yellow)
            {
                double speedChange = CalculateSpeedChange(Vehicle.Velocity, MaxSpeed,
                    edgeExploreResult.IntersectionDistance, 0, 0);

                if (speedChange <= Vehicle.MaxDeceleration && speedChange < biggestDeceleration)
                    biggestDeceleration = speedChange;
            }
            else if (edgeExploreResult.LightPhase == TrafficLightPhase.Red)
            {
                double speedChange = CalculateSpeedChange(Vehicle.Velocity, MaxSpeed,
                    edgeExploreResult.IntersectionDistance, 0, 0);
                if (speedChange < biggestDeceleration)
                    biggestDeceleration = speedChange;
            }
        }

        return biggestDeceleration;
    }

    protected virtual double CalculateSpeedChange(double currentSpeed, double maxSpeed,
        double distanceToVehicleAhead,
        double speedVehicleAhead, double accelerationVehicleAhead)
    {
        return VehicleAccelerator.CalculateSpeedChange(currentSpeed, maxSpeed, distanceToVehicleAhead,
            speedVehicleAhead);
    }

    private double HandleVehiclesAhead(SpatialGraphExploreResult exploreResult, double biggestDeceleration)
    {
        if (!Vehicle.IsCollidingEntity) return biggestDeceleration;

        // TODO nicht Spurwechsel, wenn Abbiegevorgang ansteht
        (ISpatialGraphEntity? entityAhead, double distanceToEntityAhead) = FindEntityAhead(exploreResult, Route);
        if (entityAhead == null) return biggestDeceleration;

        RoadUser vehicleAhead = entityAhead is RoadUser vehicle ? vehicle : new RoadBlocker(entityAhead);

        double speedChange = CalculateSpeedChange(Vehicle.Velocity, MaxSpeed, distanceToEntityAhead,
            vehicleAhead.Velocity, vehicleAhead.Acceleration);

        if (!DesireToOvertake(speedChange)) return Math.Min(speedChange, biggestDeceleration);

        int leftIndex = Vehicle.LaneOnCurrentEdge - 1;
        int rightIndex = Vehicle.LaneOnCurrentEdge + 1;

        EdgeExploreResult? edgeExploreResult = exploreResult.EdgeExplores.First();
        (int minLane, int maxLane) = edgeExploreResult.Edge.ModalityLaneRanges[Vehicle.ModalityType];
        bool hasLeftLane = edgeExploreResult.LaneExplores.ContainsKey(leftIndex) && leftIndex >= minLane &&
                           leftIndex <= maxLane; // TODO test?
        bool hasRightLane = edgeExploreResult.LaneExplores.ContainsKey(rightIndex) && rightIndex >= minLane &&
                            rightIndex <= maxLane;

        (ISpatialGraphEntity? entityAheadLeft, double distanceAheadLeft) = FindEntityOnSameEdge(exploreResult, leftIndex, true);
        (ISpatialGraphEntity? entityAheadRight, double distanceAheadRight) = FindEntityOnSameEdge(exploreResult, rightIndex, true);

        (ISpatialGraphEntity entityBehindLeft, double distanceBehindLeft) = FindEntityOnSameEdge(exploreResult, leftIndex, false);
        (ISpatialGraphEntity entityBehindRight, double distanceBehindRight) = FindEntityOnSameEdge(exploreResult, rightIndex, false);

        int nextLaneRelative = 0;
        if (hasLeftLane && (entityAheadLeft == null || distanceAheadLeft > distanceToEntityAhead) &&
            OvertakingIsSafelyPossible(entityBehindLeft, distanceBehindLeft))
            nextLaneRelative = -1;

        if (hasRightLane && (entityAheadRight == null || distanceAheadRight > distanceToEntityAhead) &&
            OvertakingIsSafelyPossible(entityBehindRight, distanceBehindRight))
        {
            bool switchingLeftNotPossible = nextLaneRelative == 0;
            if (switchingLeftNotPossible || distanceAheadRight > distanceAheadLeft)
                nextLaneRelative = 1;
        }

        if (nextLaneRelative != 0) PlanDesiredLanesForNextMoves(Vehicle.LaneOnCurrentEdge + nextLaneRelative);

        return biggestDeceleration;
    }

    private bool OvertakingIsSafelyPossible(ISpatialGraphEntity entityBehind, double distanceToEntityBehind)
    {
        // TODO also check that not a driver from behind is fast joining up
        if (entityBehind == null) return true;
        double velocity = entityBehind is RoadUser vehicle ? vehicle.Velocity : 0;
        bool slower = Velocity > velocity;

        const int safetyTimeInSeconds = 10;
        double speedDifference = Velocity - velocity;
        double safetyDistance = speedDifference * safetyTimeInSeconds;
        if (slower) return distanceToEntityBehind > safetyDistance;

        return distanceToEntityBehind < safetyDistance + SafetyDistanceForOvertaking;
    }

    private bool DesireToOvertake(double speedChange)
    {
        (int minLane, int maxLane) = Vehicle.CurrentEdge.ModalityLaneRanges[Vehicle.ModalityType];
        bool multiLaneRoad = maxLane - minLane >= 1;
        return Vehicle.Driver.OvertakingActivated && multiLaneRoad && speedChange < 0;
    }

    protected virtual double CalculateDrivingDistance(double biggestDeceleration)
    {
        // TODO magic number, maybe smaller?
        if (Route.RemainingRouteDistanceToGoal < 3) // Make last step to goal
            return Route.RemainingRouteDistanceToGoal;

        if (biggestDeceleration < MaximalDeceleration) // Was speed change set? then use it
            return Vehicle.Velocity + biggestDeceleration;

        // free way driving
        double speedChange = CalculateSpeedChange(Vehicle.Velocity, MaxSpeed,
            FreeDrivingClearanceInM, SpeedLimit, 0);
        return Vehicle.Velocity + speedChange;
    }

    private void PerformMoveAction(double distance)
    {
        if (distance > 0)
        {
            if (Environment.Move(Vehicle, Route, distance))
            {
                Vehicle.Velocity = Math.Round(distance, 2);
                Vehicle.Acceleration = distance - Vehicle.Velocity;
                Vehicle.Position = Vehicle.CalculateNewPositionFor(Route, out double bearing);
                Vehicle.Bearing = bearing;
            }

            if (Vehicle.CurrentEdge != Route.Stops.First().Edge) PlanDesiredLanesForNextMoves();
        }
        else
        {
            Vehicle.Acceleration = -Vehicle.Velocity; // TODO really?
            Vehicle.Velocity = 0;
        }

        if (GoalReached) Vehicle.Velocity = 0;
    }

    /// <summary>
    ///     Tries to find an entity ahead for given route within the explore result.
    /// </summary>
    /// <param name="exploreResult">Contains the exploration of the vehicle.</param>
    /// <param name="route">Holds the lanes for all relevant edges.</param>
    /// <returns>
    ///     The next entity on any of the upcoming lanes and its distance to the calling vehicle.
    ///     Null (and 0 as distance) if no entity could be found within the exploration distance.
    /// </returns>
    public (ISpatialGraphEntity, double) FindEntityAhead(SpatialGraphExploreResult exploreResult, Route route)
    {
        double distance = Vehicle.CurrentEdge != null ? -Vehicle.PositionOnCurrentEdge : 0;
        for (int i = 0; i < exploreResult.EdgeExplores.Count; i++)
        {
            EdgeExploreResult? edgeExplore = exploreResult.EdgeExplores[i];
            int desiredLane = Math.Max(0, route[i].DesiredLane);

            if (edgeExplore.LaneExplores.TryGetValue(desiredLane, out LaneExploreResult? laneExploreResult))
            {
                ISpatialGraphEntity? entity = laneExploreResult.Forward.FirstOrDefault();
                if (entity != null)
                {
                    distance += entity.PositionOnCurrentEdge;
                    return (entity, distance);
                }
            }

            distance += edgeExplore.Edge.Length;
        }

        return (null, 0);
    }

    /// <summary>
    ///     Finds an entity and the distance to this vehicle within the current edge on a specific lane if it exists.
    /// </summary>
    /// <param name="exploreResult">Holds exploration data.</param>
    /// <param name="lane">On that the searched entity may be located.</param>
    /// <param name="forward">True, if searching in front, false if searching backwards.</param>
    /// <returns>The found entity and the relative distance to this vehicle. (null, 0) if none is found.</returns>
    public (ISpatialGraphEntity, double) FindEntityOnSameEdge(SpatialGraphExploreResult exploreResult, int lane,
        bool forward)
    {
        EdgeExploreResult? edgeExploreResult = exploreResult.EdgeExplores.FirstOrDefault();
        if (edgeExploreResult == null || !edgeExploreResult.LaneExplores.ContainsKey(lane)) return (null, 0);

        double distance = Vehicle.CurrentEdge != null ? -Vehicle.PositionOnCurrentEdge : 0;
        LaneExploreResult? laneExplore = edgeExploreResult.LaneExplores[lane];
        ISpatialGraphEntity? entity = (forward ? laneExplore.Forward : laneExplore.Backward).FirstOrDefault();
        if (entity == null) return (null, 0);

        distance += entity.PositionOnCurrentEdge;
        return (entity, distance);
    } // TODO write test cases


    protected void ChangeLane()
    {
        if (Route.Stops[0].DesiredLane > Vehicle.LaneOnCurrentEdge &&
            ((SpatialEdge)Route.Stops[0].Edge).LaneCount > Route.Stops[0].DesiredLane)
            Route.Stops[0].DesiredLane += 1;

        if (Route.Stops[0].DesiredLane < Vehicle.LaneOnCurrentEdge && Route.Stops[0].DesiredLane >= 0)
            Route.Stops[0].DesiredLane -= 1;

        if (!Environment.Move(Vehicle, Route, 0.1)) Route.Stops[0].DesiredLane = Vehicle.LaneOnCurrentEdge;
    }

    protected bool MoveFromNodeSuccessfully()
    {
        if (Route.Stops.Count == 0)
            throw new ArgumentOutOfRangeException(nameof(Route), "Route was empty");

        PlanDesiredLanesForNextMoves();

        Vehicle.Acceleration = 0.0;

        return Environment.Move(Vehicle, Route, 0.001);
    }

    private void PlanDesiredLanesForNextMoves(int desiredLane = -1)
    {
        if (desiredLane >= 0) Route.First().DesiredLane = desiredLane;

        const int maxLanesAhead = 5;
        bool firstLaneInitRequired = desiredLane < 0 && Vehicle.LaneOnCurrentEdge == -1;
        for (int i = firstLaneInitRequired ? 0 : 1;
             i < maxLanesAhead && i < Route.Stops.Count - 1 && ((SpatialEdge)Route.Stops[i].Edge).LaneCount > 1;
             i++)
        {
            EdgeStop? currentStop = Route.Stops[i];
            (int leftLane, int rightLane) = currentStop.Edge.ModalityLaneRanges[Vehicle.ModalityType];
            if (currentStop.DesiredLane == -1 || desiredLane != -1)
            {
                int currentLane = desiredLane < 0 ? leftLane : desiredLane;
                EdgeStop? nextStop = Route.Stops[i + 1];
                double incoming = currentStop.Edge.From.Position.GetBearing(currentStop.Edge.To.Position);
                double outgoing = nextStop.Edge.From.Position.GetBearing(nextStop.Edge.To.Position);

                DirectionType direction = PositionHelper.GetDirectionType(incoming, outgoing);
                switch (direction)
                {
                    case DirectionType.Up:
                        currentStop.DesiredLane = rightLane > currentLane ? currentLane : rightLane;
                        break;
                    case DirectionType.Left:
                    case DirectionType.UpLeft:
                    case DirectionType.DownLeft:
                        currentStop.DesiredLane = leftLane;
                        break;
                    case DirectionType.Right:
                    case DirectionType.UpRight:
                    case DirectionType.DownRight:
                        currentStop.DesiredLane = rightLane;
                        break;
                    case DirectionType.Down:
                        currentStop.DesiredLane = leftLane;
                        break;
                    default:
                        currentStop.DesiredLane = leftLane;
                        break;
                }
            }
        }
    }
}

internal sealed class RoadBlocker : RoadUser
{
    public RoadBlocker(ISpatialGraphEntity entityAhead)
    {
        Velocity = 0;
        Acceleration = 0;
        CurrentEdge = entityAhead.CurrentEdge;
        PositionOnCurrentEdge = entityAhead.PositionOnCurrentEdge;
        LaneOnCurrentEdge = entityAhead.LaneOnCurrentEdge;
    }
}