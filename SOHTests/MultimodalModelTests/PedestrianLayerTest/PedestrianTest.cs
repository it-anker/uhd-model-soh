using System.Linq;
using Mars.Components.Environments;
using Mars.Interfaces.Environments;
using SOHModel.Domain.Common;
using SOHModel.Domain.Steering.Handles;
using SOHModel.Multimodal.Model;
using SOHModel.Multimodal.Multimodal;
using SOHModel.Multimodal.Routing;
using SOHTests.Commons.Agent;
using SOHTests.Commons.Environment;
using SOHTests.Commons.Layer;
using Xunit;

namespace SOHTests.MultimodalModelTests.PedestrianLayerTest;

public class PedestrianTest
{
    [Fact]
    public void ChangeWalkToRun()
    {
        FourNodeGraphEnv environment = new FourNodeGraphEnv();
        TestMultimodalLayer multimodalLayer = new TestMultimodalLayer(environment.GraphEnvironment);
        TestPedestrian pedestrian = new TestPedestrian
        {
            StartPosition = FourNodeGraphEnv.Node1Pos
        };
        pedestrian.Init(multimodalLayer);
        Assert.Equal(GenderType.Male, pedestrian.Gender);

        Route? route = environment.GraphEnvironment.FindRoute(environment.Node1, environment.Node2);
        pedestrian.MultimodalRoute = new MultimodalRoute(route, ModalChoice.Walking);

        pedestrian.SetRunning();
        Assert.InRange(pedestrian.PreferredSpeed,
            HumanVelocityConstants.MeanValueRunMale - HumanVelocityConstants.DeviationRunMale,
            HumanVelocityConstants.MeanValueRunMale + HumanVelocityConstants.DeviationRunMale);
        pedestrian.Tick();
        Assert.InRange(pedestrian.Velocity,
            HumanVelocityConstants.MeanValueRunMale - HumanVelocityConstants.DeviationRunMale,
            HumanVelocityConstants.MeanValueRunMale + HumanVelocityConstants.DeviationRunMale);

        pedestrian.SetWalking();
        Assert.InRange(pedestrian.PreferredSpeed,
            HumanVelocityConstants.MeanValueWalkMale - HumanVelocityConstants.DeviationWalkMale,
            HumanVelocityConstants.MeanValueWalkMale + HumanVelocityConstants.DeviationWalkMale);
        pedestrian.Tick();
        Assert.InRange(pedestrian.Velocity,
            HumanVelocityConstants.MeanValueWalkMale - HumanVelocityConstants.DeviationWalkMale,
            HumanVelocityConstants.MeanValueWalkMale + HumanVelocityConstants.DeviationWalkMale);
    }

    [Fact]
    public void InvalidateActiveSteeringOnLeavingSidewalk()
    {
        FourNodeGraphEnv environment = new FourNodeGraphEnv();
        TestMultimodalLayer multimodalLayer = new TestMultimodalLayer(environment.GraphEnvironment);
        ISpatialNode startNode = environment.Node1;
        ISpatialNode goalNode = environment.Node2;

        TestPedestrian pedestrian = new TestPedestrian
        {
            StartPosition = FourNodeGraphEnv.Node1Pos
        };
        pedestrian.Init(multimodalLayer);
        Assert.Null(pedestrian.ActiveSteeringHandle);
        Assert.False(pedestrian.OnSidewalk);

        Route? route = environment.GraphEnvironment.FindRoute(startNode, goalNode);
        pedestrian.MultimodalRoute = new MultimodalRoute(route, ModalChoice.Walking);
        pedestrian.Tick();
        Assert.NotNull(pedestrian.ActiveSteeringHandle.Route);

        pedestrian.EnterModalType(pedestrian.MultimodalRoute.CurrentModalChoice,
            pedestrian.MultimodalRoute.CurrentRoute);
        Assert.True(pedestrian.OnSidewalk);

        pedestrian.LeaveModalType(pedestrian.MultimodalRoute.CurrentModalChoice);
        Assert.Null(pedestrian.ActiveSteeringHandle);
        Assert.False(pedestrian.OnSidewalk);
    }

    [Fact]
    public void MoveFromInitialNodeSuccessfully()
    {
        FourNodeGraphEnv environment = new FourNodeGraphEnv();
        TestMultimodalLayer multimodalLayer = new TestMultimodalLayer(environment.GraphEnvironment);
        ISpatialNode startNode = environment.Node1;
        ISpatialNode goalNode = environment.Node2;

        TestPedestrian pedestrian = new TestPedestrian
        {
            StartPosition = startNode.Position
        };
        pedestrian.Init(multimodalLayer);

        Route? route = environment.GraphEnvironment.FindRoute(startNode, goalNode, (_, edge, _) => edge.Length);
        pedestrian.MultimodalRoute = new MultimodalRoute(route, ModalChoice.Walking);

        Assert.Equal(0, pedestrian.Velocity);
        Assert.Equal(startNode.Position, pedestrian.Position);

        pedestrian.Move();

        Assert.True(pedestrian.Velocity > 0);
        Assert.NotEqual(startNode.Position, pedestrian.Position);
    }

    [Fact]
    public void MoveFromNode1ToNode2()
    {
        FourNodeGraphEnv environment = new FourNodeGraphEnv();
        TestMultimodalLayer multimodalLayer = new TestMultimodalLayer(environment.GraphEnvironment);
        ISpatialNode startNode = environment.Node1;
        ISpatialNode goalNode = environment.Node2;

        TestPedestrian pedestrian = new TestPedestrian
        {
            StartPosition = startNode.Position
        };
        pedestrian.Init(multimodalLayer);

        Route? route = environment.GraphEnvironment.FindRoute(startNode, goalNode);
        pedestrian.MultimodalRoute = new MultimodalRoute(route, ModalChoice.Walking);

        pedestrian.Move();
        Assert.NotEqual(startNode.Position, pedestrian.Position);

        for (int tick = 0; tick < 200 && !pedestrian.GoalReached; tick++) pedestrian.Move();

        Assert.Equal(goalNode.Position.X, pedestrian.Position.X, 3);
        Assert.Equal(goalNode.Position.Y, pedestrian.Position.Y, 3);
    }

    [Fact]
    public void PositionCorrectAfterEnteringAndLeavingSidewalk()
    {
        FourNodeGraphEnv environment = new FourNodeGraphEnv();
        TestMultimodalLayer multimodalLayer = new TestMultimodalLayer(environment.GraphEnvironment);
        Position startPosition = FourNodeGraphEnv.Node1Pos;

        TestPedestrian pedestrian = new TestPedestrian
        {
            StartPosition = startPosition
        };
        pedestrian.Init(multimodalLayer);
        Assert.Null(pedestrian.ActiveSteeringHandle);
        Assert.Equal(startPosition, pedestrian.Position);
        Assert.Equal(Whereabouts.Offside, pedestrian.Whereabouts);

        Route? route = environment.GraphEnvironment.FindShortestRoute(environment.Node1, environment.Node2);
        pedestrian.MultimodalRoute = new MultimodalRoute(route, ModalChoice.Walking);
        Assert.Null(pedestrian.ActiveSteeringHandle);
        Assert.Equal(startPosition, pedestrian.Position);
        Assert.Equal(Whereabouts.Offside, pedestrian.Whereabouts);

        pedestrian.EnterModalType(pedestrian.MultimodalRoute.CurrentModalChoice,
            pedestrian.MultimodalRoute.CurrentRoute);
        Assert.NotNull(pedestrian.ActiveSteeringHandle);
        Assert.Equal(startPosition, pedestrian.Position);
        Assert.Equal(Whereabouts.Sidewalk, pedestrian.Whereabouts);

        pedestrian.LeaveModalType(pedestrian.MultimodalRoute.CurrentModalChoice);
        Assert.Null(pedestrian.ActiveSteeringHandle);
        Assert.Equal(startPosition, pedestrian.Position);
        Assert.Equal(Whereabouts.Offside, pedestrian.Whereabouts);
    }

    [Fact]
    public void PositionParameterIsUpdated()
    {
        FourNodeGraphEnv environment = new FourNodeGraphEnv();
        TestMultimodalLayer multimodalLayer = new TestMultimodalLayer(environment.GraphEnvironment);
        ISpatialNode node1 = environment.Node1;
        ISpatialNode node2 = environment.Node2;
        TestPedestrian pedestrian = new TestPedestrian
        {
            StartPosition = node1.Position
        };
        pedestrian.Init(multimodalLayer);
        Assert.Equal(node1.Position, pedestrian.Position);

        Route? route = environment.GraphEnvironment.FindRoute(node1, node2, (_, edge, _) => edge.Length);
        pedestrian.MultimodalRoute = new MultimodalRoute(route, ModalChoice.Walking);

        Assert.Null(pedestrian.ActiveSteeringHandle);
        Assert.Equal(node1.Position, pedestrian.Position);

        pedestrian.Move();
        Assert.NotNull(pedestrian.ActiveSteeringHandle);
        Assert.True(pedestrian.ActiveSteeringHandle is WalkingSteeringHandle);
        Assert.NotEqual(node1.Position, pedestrian.Position);
    }

    [Fact]
    public void PositionPreservedAfterMoveAndLeavingSidewalk()
    {
        FourNodeGraphEnv environment = new FourNodeGraphEnv();
        TestMultimodalLayer multimodalLayer = new TestMultimodalLayer(environment.GraphEnvironment);
        ISpatialNode startNode = environment.Node1;
        ISpatialNode goalNode = environment.Node2;

        TestPedestrian pedestrian = new TestPedestrian
        {
            StartPosition = startNode.Position
        };
        pedestrian.Init(multimodalLayer);
        Assert.Equal(startNode.Position, pedestrian.Position);

        Route? route = environment.GraphEnvironment.FindRoute(startNode, goalNode);
        pedestrian.MultimodalRoute = new MultimodalRoute(route, ModalChoice.Walking);
        Assert.Equal(startNode.Position, pedestrian.Position);

        pedestrian.Move();
        Position currentPosition = pedestrian.Position;

        // pedestrian.LeaveSidewalk();
        Assert.Equal(currentPosition, pedestrian.Position);
    }

    [Fact]
    public void ResetRoadUserParameters()
    {
        FourNodeGraphEnv environment = new FourNodeGraphEnv();
        TestMultimodalLayer multimodalLayer = new TestMultimodalLayer(environment.GraphEnvironment);
        ISpatialNode startNode = environment.Node1;
        TestPedestrian pedestrian = new TestPedestrian
        {
            StartPosition = startNode.Position
        };
        pedestrian.Init(multimodalLayer);
        Assert.Null(pedestrian.CurrentEdge);
        Assert.Equal(0, pedestrian.PositionOnCurrentEdge);

        Route? route = environment.GraphEnvironment.FindRoute(startNode, environment.Node2);
        pedestrian.MultimodalRoute = new MultimodalRoute(route, ModalChoice.Walking);
        pedestrian.Move();
        Assert.NotNull(pedestrian.CurrentEdge);
        Assert.NotEqual(0, pedestrian.PositionOnCurrentEdge);

        pedestrian.LeaveModalType(pedestrian.MultimodalRoute.CurrentModalChoice);
        Assert.Null(pedestrian.CurrentEdge);
        Assert.Equal(0, pedestrian.PositionOnCurrentEdge);
    }

    [Fact]
    public void RunningAgentOvertakesWalkingAgent()
    {
        FourNodeGraphEnv environment = new FourNodeGraphEnv();
        TestMultimodalLayer multimodalLayer = new TestMultimodalLayer(environment.GraphEnvironment);
        Assert.True(HumanVelocityConstants.MeanValueRunMale - HumanVelocityConstants.DeviationRunMale >
                    HumanVelocityConstants.MeanValueWalkMale + HumanVelocityConstants.DeviationWalkMale);

        TestPedestrian walkingAgent = new TestPedestrian
        {
            StartPosition = FourNodeGraphEnv.Node1Pos
        };
        walkingAgent.Init(multimodalLayer);
        TestPedestrian runningAgent = new TestPedestrian
        {
            StartPosition = FourNodeGraphEnv.Node1Pos
        };
        runningAgent.Init(multimodalLayer);

        ISpatialGraphEnvironment graphEnvironment = environment.GraphEnvironment;
        walkingAgent.MultimodalRoute =
            new MultimodalRoute(graphEnvironment.FindRoute(environment.Node1, environment.Node2),
                ModalChoice.Walking);
        runningAgent.MultimodalRoute =
            new MultimodalRoute(graphEnvironment.FindRoute(environment.Node1, environment.Node2),
                ModalChoice.Walking);


        walkingAgent.SetWalking();
        runningAgent.SetRunning();

        Assert.Equal(FourNodeGraphEnv.Node1Pos, walkingAgent.Position);
        Assert.Equal(FourNodeGraphEnv.Node1Pos, runningAgent.Position);

        walkingAgent.Tick(); // move first
        Assert.NotEqual(FourNodeGraphEnv.Node1Pos, walkingAgent.Position);

        for (int tick = 0; tick < 5; tick++)
        {
            walkingAgent.Move();
            runningAgent.Move();
        }

        Assert.True(runningAgent.Velocity > walkingAgent.Velocity);
        Assert.True(runningAgent.PositionOnCurrentEdge > walkingAgent.PositionOnCurrentEdge);
    }

    [Fact]
    public void SwitchRouteOnTheWay()
    {
        FourNodeGraphEnv fourNodeEnv = new FourNodeGraphEnv();
        ISpatialGraphEnvironment env = fourNodeEnv.GraphEnvironment;
        TestMultimodalLayer multimodalLayer = new TestMultimodalLayer(fourNodeEnv.GraphEnvironment);
        TestPedestrian pedestrian = new TestPedestrian
        {
            StartPosition = fourNodeEnv.Node1.Position
        };
        pedestrian.Init(multimodalLayer);

        Route? route = env.FindRoute(fourNodeEnv.Node1, fourNodeEnv.Node4, (_, edge, _) => edge.Length);
        pedestrian.MultimodalRoute = new MultimodalRoute(route, ModalChoice.Walking);

        bool onWayToNode4 = true;
        for (int tick = 0; tick < 10000 && !pedestrian.GoalReached; tick++)
        {
            pedestrian.Move();

            if (onWayToNode4 && route.First().Edge.To.Equals(fourNodeEnv.Node4))
            {
                Route? routeToNode3 = env.FindRoute(fourNodeEnv.Node1, fourNodeEnv.Node3,
                    (_, edge, _) => edge.Length);
                Assert.InRange(route.RouteLength - route.RemainingRouteDistanceToGoal, routeToNode3.RouteLength,
                    route.RouteLength);

                // go back
                onWayToNode4 = false;
                ISpatialNode? currentNode = route.First().Edge.From;
                route = env.FindRoute(currentNode, fourNodeEnv.Node1, (_, edge, _) => edge.Length);
                pedestrian.MultimodalRoute = new MultimodalRoute(route, ModalChoice.Walking);
            }
        }

        Assert.True(pedestrian.GoalReached);
        Assert.InRange(fourNodeEnv.Node1.Position.DistanceInKmTo(pedestrian.Position), 0, 0.005);
        Assert.Equal(fourNodeEnv.Node1.Position.Latitude, pedestrian.Position.Latitude, 3);
        Assert.Equal(fourNodeEnv.Node1.Position.Longitude, pedestrian.Position.Longitude, 3);
    }

    [Fact]
    public void SwitchRouteWithinMoveProceedOnSameEdge()
    {
        FourNodeGraphEnv fourNodeGraphEnv = new FourNodeGraphEnv();
        ISpatialGraphEnvironment environment = fourNodeGraphEnv.GraphEnvironment;

        TestMultimodalLayer multimodalLayer = new TestMultimodalLayer(environment);
        ISpatialNode startNode = fourNodeGraphEnv.Node1;

        TestPedestrian pedestrian = new TestPedestrian
        {
            StartPosition = startNode.Position
        };
        pedestrian.Init(multimodalLayer);
        Assert.Null(pedestrian.ActiveSteeringHandle);
        Assert.Null(pedestrian.CurrentEdge);
        Assert.False(pedestrian.OnSidewalk);

        Route? firstRoute = environment.FindRoute(startNode, fourNodeGraphEnv.Node2, (_, edge, _) => edge.Length);
        pedestrian.MultimodalRoute = new MultimodalRoute(firstRoute, ModalChoice.Walking);
        pedestrian.Move();
        Assert.NotNull(pedestrian.CurrentEdge);
        Assert.NotEqual(0, pedestrian.PositionOnCurrentEdge);

        Route? secondRoute = environment.FindRoute(startNode, fourNodeGraphEnv.Node3, (_, edge, _) => edge.Length);
        Assert.Equal(secondRoute.First().Edge, pedestrian.CurrentEdge);

        pedestrian.MultimodalRoute = new MultimodalRoute(secondRoute, ModalChoice.Walking);
        double oldPositionOnCurrentEdge = pedestrian.PositionOnCurrentEdge;
        pedestrian.Move();

        Assert.Equal(firstRoute.First().Edge, pedestrian.CurrentEdge);
        Assert.Equal(secondRoute.First().Edge, pedestrian.CurrentEdge);
        Assert.InRange(pedestrian.PositionOnCurrentEdge, oldPositionOnCurrentEdge, firstRoute.First().Edge.Length);
    }

    [Fact]
    public void SwitchRouteWithinMoveRequiresJump()
    {
        FourNodeGraphEnv environment = new FourNodeGraphEnv();
        TestMultimodalLayer multimodalLayer = new TestMultimodalLayer(environment.GraphEnvironment);
        ISpatialNode startNode = environment.Node1;
        TestPedestrian pedestrian = new TestPedestrian
        {
            StartPosition = startNode.Position
        };
        pedestrian.Init(multimodalLayer);
        Assert.Null(pedestrian.ActiveSteeringHandle);
        Assert.False(pedestrian.OnSidewalk);

        Route? firstRoute = environment.GraphEnvironment.FindRoute(startNode, environment.Node2);
        pedestrian.MultimodalRoute = new MultimodalRoute(firstRoute, ModalChoice.Walking);
        pedestrian.Move();
        Assert.NotNull(pedestrian.ActiveSteeringHandle);
        Assert.NotNull(pedestrian.CurrentEdge);
        Assert.NotEqual(0, pedestrian.PositionOnCurrentEdge);

        Route? secondRoute = environment.GraphEnvironment.FindRoute(environment.Node2, environment.Node3);
        Assert.NotEqual(secondRoute.First().Edge, pedestrian.CurrentEdge);

        pedestrian.MultimodalRoute = new MultimodalRoute(secondRoute, ModalChoice.Walking);
        pedestrian.Move();

        //jump to new route
        Assert.Equal(secondRoute.First().Edge, pedestrian.CurrentEdge);
        Assert.NotEqual(firstRoute.First().Edge, pedestrian.CurrentEdge);
        Assert.NotEqual(0, pedestrian.PositionOnCurrentEdge);
    }

    [Fact]
    public void WalkFullFourNodeEnvironment()
    {
        FourNodeGraphEnv environment = new FourNodeGraphEnv();
        TestMultimodalLayer layer = new TestMultimodalLayer(environment.GraphEnvironment);
        Position start = FourNodeGraphEnv.Node1Pos;
        Position? goal = environment.Node4.Position;
        TestPedestrian pedestrian = new TestPedestrian
        {
            StartPosition = start
        };
        pedestrian.Init(layer);
        // pedestrian.TryEnterVehicleAsDriver(this);
        // pedestrian.EnterSidewalk(pedestrian.Position);
        Assert.Equal(start, pedestrian.Position);

        pedestrian.MultimodalRoute = new WalkingMultimodalRoute(layer.SpatialGraphMediatorLayer, start, goal);

        for (int tick = 0; tick < 5000 && !pedestrian.GoalReached; tick++, layer.Context.UpdateStep())
            pedestrian.Tick();
        Assert.True(pedestrian.GoalReached);

        Assert.InRange(goal.DistanceInMTo(pedestrian.Position), 0, 3);
        Assert.Equal(goal, pedestrian.Position);
    }

    [Fact]
    public void WalkToReachNodeOnAltonaGraph()
    {
        SpatialGraphEnvironment environment = new SpatialGraphEnvironment(ResourcesConstants.DriveGraphAltonaAltstadt);
        TestMultimodalLayer multimodalLayer = new TestMultimodalLayer(environment);

        ISpatialNode start = environment.NearestNode(Position.CreateGeoPosition(9.845780, 53.570825));
        ISpatialNode goal = environment.NearestNode(Position.CreateGeoPosition(9.847038, 53.571780));

        TestPedestrian pedestrian = new TestPedestrian
        {
            StartPosition = start.Position
        };
        pedestrian.Init(multimodalLayer);

        Assert.Equal(start.Position, pedestrian.Position);
        Assert.Equal(0, pedestrian.PositionOnCurrentEdge);
        Assert.Null(pedestrian.CurrentEdge);

        Route route = environment.FindRoute(start, goal);
        pedestrian.MultimodalRoute = new MultimodalRoute(route, ModalChoice.Walking);

        for (int tick = 0; tick < 100; tick++)
        {
            pedestrian.Move();
            if (pedestrian.GoalReached) break;
        }

        Assert.True(pedestrian.GoalReached);
        Assert.InRange(goal.Position.DistanceInKmTo(pedestrian.Position), 0, 001);
        Assert.Equal(goal.Position.Latitude, pedestrian.Position.Latitude, 10);
        Assert.Equal(goal.Position.Longitude, pedestrian.Position.Longitude, 10);
    }

    [Fact]
    public void WalkToReachNodeOnSimpleEnvironment()
    {
        FourNodeGraphEnv environment = new FourNodeGraphEnv();
        TestMultimodalLayer multimodalLayer = new TestMultimodalLayer(environment.GraphEnvironment);

        TestPedestrian pedestrian = new TestPedestrian
        {
            StartPosition = FourNodeGraphEnv.Node1Pos
        };
        pedestrian.Init(multimodalLayer);

        Assert.Equal(FourNodeGraphEnv.Node1Pos, pedestrian.Position);
        Assert.Equal(0, pedestrian.PositionOnCurrentEdge);
        Assert.Null(pedestrian.CurrentEdge);

        pedestrian.Move();

        //no change, because of missing route
        Assert.Equal(FourNodeGraphEnv.Node1Pos, pedestrian.Position);
        Assert.Equal(0, pedestrian.PositionOnCurrentEdge);
        Assert.Null(pedestrian.CurrentEdge);

        Route? route = environment.GraphEnvironment.FindRoute(environment.Node1, environment.Node2);
        pedestrian.MultimodalRoute = new MultimodalRoute(route, ModalChoice.Walking);

        //no change without movement
        Assert.Equal(FourNodeGraphEnv.Node1Pos, pedestrian.Position);
        Assert.Equal(0, pedestrian.PositionOnCurrentEdge);
        Assert.Null(pedestrian.CurrentEdge);

        for (int tick = 0; tick < 10000; tick++)
        {
            pedestrian.Move();
            if (pedestrian.GoalReached) break;
        }

        Assert.True(pedestrian.GoalReached);
        Assert.InRange(environment.Node2.Position.DistanceInKmTo(pedestrian.Position), 0, 001);
        Assert.Equal(environment.Node2.Position.Latitude, pedestrian.Position.Latitude, 2);
        Assert.Equal(environment.Node2.Position.Longitude, pedestrian.Position.Longitude, 2);
    }

    private class TestPedestrian : TestMultiCapableAgent
    {
        public ISteeringHandle ActiveSteeringHandle => ActiveSteering;

        public new bool OnSidewalk => base.OnSidewalk;
        public double PositionOnCurrentEdge => WalkingShoes.PositionOnCurrentEdge;
        public ISpatialEdge CurrentEdge => WalkingShoes.CurrentEdge;

        public new void EnterModalType(ModalChoice modalChoice, Route route)
        {
            base.EnterModalType(modalChoice, route);
        }

        public new void LeaveModalType(ModalChoice modalChoice)
        {
            base.LeaveModalType(modalChoice);
        }

        public override void Tick()
        {
            Move();
        }
    }
}