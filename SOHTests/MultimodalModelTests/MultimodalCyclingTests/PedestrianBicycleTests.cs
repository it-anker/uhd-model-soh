using System.Collections.Generic;
using System.Linq;
using Mars.Components.Environments;
using Mars.Interfaces;
using Mars.Interfaces.Environments;
using Mars.Interfaces.Model;
using Mars.Interfaces.Model.Options;
using SOHModel.Bicycle.Rental;
using SOHModel.Car.Model;
using SOHModel.Multimodal.Multimodal;
using SOHTests.Commons.Agent;
using SOHTests.Commons.Environment;
using SOHTests.Commons.Layer;
using Xunit;

namespace SOHTests.MultimodalModelTests.MultimodalCyclingTests;

public class PedestrianBicycleTests
{
    private static void CompareCyclistAndPedestrian(TestMultimodalLayer multimodalLayer, Position start,
        Position goal, bool pedestrianArrivesFirst)
    {
        ISimulationContext simulationContext = multimodalLayer.Context;

        TestWalkingPedestrian pedestrian = new TestWalkingPedestrian
        {
            StartPosition = start,
            GoalPosition = goal
        };
        pedestrian.Init(multimodalLayer);

        TestMultiCapableAgent cyclist = new TestMultiCapableAgent
        {
            StartPosition = start,
            GoalPosition = goal,
            ModalChoice = ModalChoice.CyclingRentalBike
        };
        cyclist.Init(multimodalLayer);

        List<MultimodalAgent<TestMultimodalLayer>> agents = new List<MultimodalAgent<TestMultimodalLayer>>
        {
            pedestrian,
            cyclist
        };
        Assert.Equal(ModalChoice.Walking, pedestrian.RouteMainModalChoice);
        Assert.Equal(ModalChoice.CyclingRentalBike, cyclist.RouteMainModalChoice);

        bool cyclistArrivedGoal = false;
        for (int tick = 0; tick < 10000 && !agents.All(agent => agent.GoalReached); tick++)
        {
            // foreach (var agent in agents) agent.Tick();
            pedestrian.Tick();
            cyclist.Tick();
            simulationContext.UpdateStep();

            if (!cyclistArrivedGoal && cyclist.GoalReached)
            {
                Assert.Equal(pedestrianArrivesFirst, pedestrian.GoalReached);
                cyclistArrivedGoal = true;
            }
        }

        Assert.True(agents.All(agent => agent.GoalReached));
        Assert.True(agents.All(agent => agent.Whereabouts == Whereabouts.Offside));

        foreach (MultimodalAgent<TestMultimodalLayer> agent in agents)
        {
            Assert.Equal(goal.Longitude, agent.Position.Longitude, 2);
            Assert.Equal(goal.Latitude, agent.Position.Latitude, 2);
        }
    }

    private static (Position, Position) FindPedestrianBlockingCyclistTour()
    {
        Position? start = Position.CreateGeoPosition(9.950500, 53.555871);
        Position? goal = Position.CreateGeoPosition(9.942655, 53.555227);
        return (start, goal);
    }

    private void BicycleIsReturned(TestMultimodalLayer layer, IBicycleRentalLayer bicycleLayer, Position start,
        Position goal)
    {
        Assert.NotNull(bicycleLayer);
        Assert.Equal(BicycleRentalStation.StandardAmount, bicycleLayer.Nearest(start, false).Count);

        TestMultiCapableAgent cyclist = new TestMultiCapableAgent
        {
            StartPosition = start,
            GoalPosition = goal,
            ModalChoice = ModalChoice.CyclingRentalBike
        };
        cyclist.Init(layer);

        BicycleRentalStation? goalBicycleParkingSpace = bicycleLayer.Nearest(goal, false);
        Assert.Equal(BicycleRentalStation.StandardAmount, goalBicycleParkingSpace.Count);

        for (int tick = 0; tick < 10000 && !cyclist.GoalReached; tick++) cyclist.Tick();

        Assert.Equal(BicycleRentalStation.StandardAmount + 1, goalBicycleParkingSpace.Count);
    }

    private void GoalReachedByBicycle(TestMultimodalLayer layer, Position start, Position goal)
    {
        TestMultiCapableAgent cyclist = new TestMultiCapableAgent
        {
            StartPosition = start,
            GoalPosition = goal,
            ModalChoice = ModalChoice.CyclingRentalBike
        };
        cyclist.Init(layer);
        for (int tick = 0; tick < 10000 && !cyclist.GoalReached; tick++, layer.Context.UpdateStep()) cyclist.Tick();

        Assert.True(cyclist.GoalReached);

        Assert.True(cyclist.HasUsedBicycle);
        Assert.Equal(goal.Longitude, cyclist.Position.Longitude, 2);
        Assert.Equal(goal.Latitude, cyclist.Position.Latitude, 2);
    }

    [Fact]
    public void BicycleIsReturnedForCycleOnly()
    {
        FourNodeGraphEnv fourNodeGraphEnv = new FourNodeGraphEnv();
        CarLayer streetLayer = new CarLayer(fourNodeGraphEnv.GraphEnvironment);
        BicycleRentalLayer bicycleLayer = new FourNodeBicycleRentalLayerFixture(streetLayer).BicycleRentalLayer;
        TestMultimodalLayer layer = new TestMultimodalLayer(fourNodeGraphEnv.GraphEnvironment, bicycleLayer)
        {
            BicycleRentalLayer = bicycleLayer
        };

        Position? start = fourNodeGraphEnv.Node2.Position;
        Position? goal = fourNodeGraphEnv.Node3.Position;

        BicycleIsReturned(layer, bicycleLayer, start, goal);
    }

    [Fact]
    public void BicycleIsReturnedForCycleWalk()
    {
        FourNodeGraphEnv fourNodeGraphEnv = new FourNodeGraphEnv();
        CarLayer streetLayer = new CarLayer(fourNodeGraphEnv.GraphEnvironment);
        BicycleRentalLayer bicycleLayer = new FourNodeBicycleRentalLayerFixture(streetLayer).BicycleRentalLayer;
        TestMultimodalLayer layer = new TestMultimodalLayer(fourNodeGraphEnv.GraphEnvironment, bicycleLayer);

        Position? start = fourNodeGraphEnv.Node2.Position;
        Position? goal = fourNodeGraphEnv.Node4.Position;

        BicycleIsReturned(layer, bicycleLayer, start, goal);
    }

    [Fact]
    public void BicycleIsReturnedForWalkCycle()
    {
        FourNodeGraphEnv fourNodeGraphEnv = new FourNodeGraphEnv();
        CarLayer streetLayer = new CarLayer(fourNodeGraphEnv.GraphEnvironment);
        BicycleRentalLayer bicycleLayer = new FourNodeBicycleRentalLayerFixture(streetLayer).BicycleRentalLayer;
        TestMultimodalLayer layer = new TestMultimodalLayer(fourNodeGraphEnv.GraphEnvironment, bicycleLayer);

        Position start = FourNodeGraphEnv.Node1Pos;
        Position goal = FourNodeGraphEnv.Node3Pos;

        BicycleIsReturned(layer, bicycleLayer, start, goal);
    }

    [Fact]
    public void BicycleIsReturnedForWalkCycleWalk()
    {
        FourNodeGraphEnv fourNodeGraphEnv = new FourNodeGraphEnv();
        CarLayer streetLayer = new CarLayer(fourNodeGraphEnv.GraphEnvironment);
        BicycleRentalLayer bicycleLayer = new FourNodeBicycleRentalLayerFixture(streetLayer).BicycleRentalLayer;
        TestMultimodalLayer layer = new TestMultimodalLayer(fourNodeGraphEnv.GraphEnvironment, bicycleLayer);

        Position start = FourNodeGraphEnv.Node1Pos;
        Position goal = FourNodeGraphEnv.Node4Pos;

        BicycleIsReturned(layer, bicycleLayer, start, goal);
    }

    [Fact]
    public void CyclistCannotSurpassPedestrianOnBlockingRoute()
    {
        // we only have one environment for both
        SpatialGraphEnvironment environment = new SpatialGraphEnvironment(ResourcesConstants.WalkGraphAltonaAltstadt);
        BicycleRentalLayer bicycleLayer = new BicycleRentalLayerFixture(environment).BicycleRentalLayer;
        TestMultimodalLayer layer = new TestMultimodalLayer(environment, bicycleLayer);

        (Position start, Position goal) = FindPedestrianBlockingCyclistTour();
        CompareCyclistAndPedestrian(layer, start, goal, true);
    }

    [Fact]
    public void CyclistCanSurpassPedestrianOnBlockingRoute()
    {
        SpatialGraphEnvironment environment = new SpatialGraphEnvironment(new SpatialGraphOptions
        {
            GraphImports = new List<Input>
            {
                new()
                {
                    File = ResourcesConstants.WalkGraphAltonaAltstadt,
                    InputConfiguration = new InputConfiguration
                        { Modalities = new HashSet<SpatialModalityType> { SpatialModalityType.Walking } }
                },
                new()
                {
                    File = ResourcesConstants.DriveGraphAltonaAltstadt,
                    InputConfiguration = new InputConfiguration
                        { Modalities = new HashSet<SpatialModalityType> { SpatialModalityType.Cycling } }
                }
            }
        });


        //we have two environments, so cyclist can surpass pedestrian
        BicycleRentalLayer bicycleLayer = new BicycleRentalLayerFixture(environment).BicycleRentalLayer;
        TestMultimodalLayer layer = new TestMultimodalLayer(environment, bicycleLayer);

        (Position start, Position goal) = FindPedestrianBlockingCyclistTour();
        CompareCyclistAndPedestrian(layer, start, goal, false);
    }

    [Fact]
    public void GoalReachedByCycleOnly()
    {
        FourNodeGraphEnv fourNodeGraphEnv = new FourNodeGraphEnv();
        CarLayer carLayer = new CarLayer(fourNodeGraphEnv.GraphEnvironment);
        BicycleRentalLayer bicycleLayer = new FourNodeBicycleRentalLayerFixture(carLayer).BicycleRentalLayer;
        TestMultimodalLayer layer = new TestMultimodalLayer(fourNodeGraphEnv.GraphEnvironment, bicycleLayer);

        Position start = FourNodeGraphEnv.Node2Pos;
        Position goal = FourNodeGraphEnv.Node3Pos;

        GoalReachedByBicycle(layer, start, goal);
    }

    [Fact]
    public void GoalReachedByCycleWalk()
    {
        FourNodeGraphEnv fourNodeGraphEnv = new FourNodeGraphEnv();
        CarLayer carLayer = new CarLayer(fourNodeGraphEnv.GraphEnvironment);
        BicycleRentalLayer bicycleLayer = new FourNodeBicycleRentalLayerFixture(carLayer).BicycleRentalLayer;
        TestMultimodalLayer layer = new TestMultimodalLayer(fourNodeGraphEnv.GraphEnvironment, bicycleLayer);

        Position start = FourNodeGraphEnv.Node2Pos;
        Position goal = FourNodeGraphEnv.Node4Pos;

        GoalReachedByBicycle(layer, start, goal);
    }

    [Fact]
    public void GoalReachedByWalkCycle()
    {
        FourNodeGraphEnv fourNodeGraphEnv = new FourNodeGraphEnv();
        CarLayer carLayer = new CarLayer(fourNodeGraphEnv.GraphEnvironment);
        BicycleRentalLayer bicycleLayer = new FourNodeBicycleRentalLayerFixture(carLayer).BicycleRentalLayer;
        TestMultimodalLayer layer = new TestMultimodalLayer(fourNodeGraphEnv.GraphEnvironment, bicycleLayer);

        Position start = FourNodeGraphEnv.Node1Pos;
        Position goal = FourNodeGraphEnv.Node3Pos;

        GoalReachedByBicycle(layer, start, goal);
    }

    [Fact]
    public void GoalReachedByWalkCycleWalk()
    {
        FourNodeGraphEnv fourNodeGraphEnv = new FourNodeGraphEnv();
        CarLayer carLayer = new CarLayer(fourNodeGraphEnv.GraphEnvironment);
        BicycleRentalLayer bicycleLayer = new FourNodeBicycleRentalLayerFixture(carLayer).BicycleRentalLayer;
        TestMultimodalLayer layer = new TestMultimodalLayer(fourNodeGraphEnv.GraphEnvironment, bicycleLayer);

        Position start = FourNodeGraphEnv.Node1Pos;
        Position goal = FourNodeGraphEnv.Node4Pos;

        GoalReachedByBicycle(layer, start, goal);
    }

    [Fact]
    public void StartIsGoal()
    {
        FourNodeGraphEnv fourNodeGraphEnv = new FourNodeGraphEnv();
        CarLayer carLayer = new CarLayer(fourNodeGraphEnv.GraphEnvironment);
        BicycleRentalLayer bicycleLayer = new FourNodeBicycleRentalLayerFixture(carLayer).BicycleRentalLayer;
        TestMultimodalLayer layer = new TestMultimodalLayer(fourNodeGraphEnv.GraphEnvironment, bicycleLayer);

        Position start = FourNodeGraphEnv.Node2Pos;
        Position goal = start;

        TestMultiCapableAgent cyclist = new TestMultiCapableAgent
        {
            StartPosition = start,
            GoalPosition = goal,
            ModalChoice = ModalChoice.CyclingRentalBike
        };
        cyclist.Init(layer);

        for (int tick = 0; tick < 10000 && !cyclist.GoalReached; tick++) cyclist.Tick();

        Assert.True(cyclist.GoalReached);

        Assert.Equal(goal.Longitude, cyclist.Position.Longitude, 2);
        Assert.Equal(goal.Latitude, cyclist.Position.Latitude, 2);
    }
}