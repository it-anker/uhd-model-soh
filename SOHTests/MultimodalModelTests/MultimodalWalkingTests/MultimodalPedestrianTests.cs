using System.Linq;
using Mars.Components.Environments;
using Mars.Interfaces;
using Mars.Interfaces.Environments;
using SOHModel.Multimodal.Model;
using SOHModel.Multimodal.Multimodal;
using SOHModel.Multimodal.Routing;
using SOHTests.Commons.Agent;
using SOHTests.Commons.Layer;
using Xunit;

namespace SOHTests.MultimodalModelTests.MultimodalWalkingTests;

public class MultimodalPedestrianTests
{
    private readonly TestMultimodalLayer _multimodalLayer;
    private readonly SpatialGraphEnvironment _sidewalk;

    public MultimodalPedestrianTests()
    {
        _sidewalk =
            new SpatialGraphEnvironment(ResourcesConstants.WalkGraphAltonaAltstadt);

        _multimodalLayer = new TestMultimodalLayer(_sidewalk);
    }

    private ISimulationContext ContextImpl => _multimodalLayer.Context;

    [Fact]
    public void WalkAndCheckSpecificRoute()
    {
        Position? start = Position.CreateGeoPosition(9.9527061, 53.5460391);
        Position? goal = Position.CreateGeoPosition(9.9348362, 53.5479922);

        TestMultiCapableAgent agent = new TestMultiCapableAgent
        {
            StartPosition = start,
            GoalPosition = goal,
            ModalChoice = ModalChoice.Walking
        };
        agent.Init(_multimodalLayer);
        agent.SetPreferredSpeed(1.2365073881673783);
        agent.MultimodalRoute =
            _multimodalLayer.RouteFinder.Search(agent, agent.Position, goal, ModalChoice.Walking);

        int routeCount = agent.MultimodalRoute.Stops.Count;
        int routeStopCount = agent.MultimodalRoute.First().Route.Stops.Count;
        Assert.Equal(1, routeCount);

        int expectedTravelTime = agent.MultimodalRoute.ExpectedTravelTime(agent);
        Assert.Equal(1156, expectedTravelTime);

        Assert.Equal(Whereabouts.Offside, agent.Whereabouts);

        for (int tick = 0; tick < 3000 && !agent.GoalReached; tick++, ContextImpl.UpdateStep())
        {
            agent.Tick();
            if (!agent.GoalReached) Assert.Equal(Whereabouts.Sidewalk, agent.Whereabouts);

            Assert.InRange(tick, 0, expectedTravelTime + 1);
        }

        Assert.True(agent.GoalReached);
        Assert.Equal(Whereabouts.Offside, agent.Whereabouts);

        Assert.Equal(routeCount, agent.MultimodalRoute.Stops.Count);
        Assert.Equal(routeStopCount, agent.MultimodalRoute.Stops.First().Route.Stops.Count);
    }

    [Fact]
    public void WalkToGoalsAndBeOffsideAfterwards()
    {
        TestMultiCapableAgent agent = new TestMultiCapableAgent
        {
            Gender = GenderType.Female,
            StartPosition = _sidewalk.GetRandomNode().Position
        };
        agent.Init(_multimodalLayer);

        for (int run = 0; run < 3; run++)
        {
            Position? goal = _sidewalk.GetRandomNode().Position;
            agent.MultimodalRoute =
                _multimodalLayer.RouteFinder.Search(agent, agent.Position, goal, ModalChoice.Walking);

            int routeCount = agent.MultimodalRoute.Stops.Count;
            int routeStopCount = agent.MultimodalRoute.First().Route.Stops.Count;
            Assert.Equal(1, routeCount);

            int expectedTravelTime = agent.MultimodalRoute.ExpectedTravelTime(agent);

            Assert.Equal(Whereabouts.Offside, agent.Whereabouts);

            for (int tick = 0; tick < 3000 && !agent.GoalReached; tick++, ContextImpl.UpdateStep())
            {
                agent.Tick();
                if (!agent.GoalReached) Assert.Equal(Whereabouts.Sidewalk, agent.Whereabouts);

                Assert.InRange(tick, 0, expectedTravelTime + 1);
            }

            Assert.True(agent.GoalReached);
            Assert.Equal(Whereabouts.Offside, agent.Whereabouts);

            Assert.Equal(routeCount, agent.MultimodalRoute.Stops.Count);
            Assert.Equal(routeStopCount, agent.MultimodalRoute.Stops.First().Route.Stops.Count);
        }
    }

    [Fact]
    public void WalkToGoalsAndSwitchGoalInBetween()
    {
        TestMultiCapableAgent agent = new TestMultiCapableAgent
        {
            Gender = GenderType.Female,
            StartPosition = _sidewalk.GetRandomNode().Position
        };
        agent.Init(_multimodalLayer);

        Position? goal = _sidewalk.GetRandomNode().Position;
        agent.MultimodalRoute =
            _multimodalLayer.RouteFinder.Search(agent, agent.Position, goal, ModalChoice.Walking);

        Assert.Equal(Whereabouts.Offside, agent.Whereabouts);

        for (int tick = 0; tick < 3000 && !agent.GoalReached; tick++, ContextImpl.UpdateStep())
        {
            agent.Tick();
            if (tick == 10 && !agent.GoalReached)
            {
                goal = _sidewalk.GetRandomNode().Position;
                agent.MultimodalRoute =
                    _multimodalLayer.RouteFinder.Search(agent, agent.Position, goal, ModalChoice.Walking);
            }
        }

        Assert.True(agent.GoalReached);
        Assert.Equal(Whereabouts.Offside, agent.Whereabouts);
    }
}