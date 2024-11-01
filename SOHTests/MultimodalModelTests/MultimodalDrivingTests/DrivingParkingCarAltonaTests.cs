using System.Collections.Generic;
using System.Linq;
using Mars.Components.Environments;
using Mars.Interfaces;
using Mars.Interfaces.Environments;
using SOHModel.Car.Model;
using SOHModel.Car.Parking;
using SOHModel.Domain.Graph;
using SOHModel.Multimodal.Multimodal;
using SOHTests.Commons.Agent;
using SOHTests.Commons.Layer;
using Xunit;

namespace SOHTests.MultimodalModelTests.MultimodalDrivingTests;

public class DrivingParkingCarAltonaTests
{
    private readonly SpatialGraphEnvironment _environment;
    private readonly CarParkingLayer _parkingLayer;

    public DrivingParkingCarAltonaTests()
    {
        _environment = new SpatialGraphEnvironment(ResourcesConstants.DriveGraphAltonaAltstadt);
        StreetLayer streetLayer = new StreetLayer { Environment = _environment };
        _parkingLayer = new CarParkingLayerFixture(streetLayer).CarParkingLayer;
    }

    private (Position, Position) FindReasonableDrivingTour()
    {
        Position? start = _environment.GetRandomNode().Position;
        while (_parkingLayer.Nearest(start).Position.DistanceInKmTo(start) > 1)
            start = _environment.GetRandomNode().Position;

        Position? goal = _environment.GetRandomNode().Position;
        while (start.DistanceInKmTo(goal) < 3 && _parkingLayer.Nearest(goal).Position.DistanceInKmTo(goal) > 1)
            goal = _environment.GetRandomNode().Position;

        return (start, goal);
    }

    [Fact]
    public void CompareRouteSearchingByLengthAndByTravelTime()
    {
        TestMultimodalLayer layer = new TestMultimodalLayer(_environment);
        ISimulationContext simulationContext = layer.Context;
        // var (start, goal) = FindReasonableDrivingTour();
        Position? start = Position.CreatePosition(9.9517443, 53.5556583);
        Position? goal = Position.CreatePosition(9.93372, 53.54580);

        TestMultiCapableCarDriver lengthDrivingAgent = new TestMultiCapableCarDriver
        {
            StartPosition = start,
            GoalPosition = goal,
            Car = Golf.CreateOnParking(_parkingLayer, _environment, start)
        };
        lengthDrivingAgent.Init(layer);
        TestMultiCapableCarDriver timeDrivingAgent = new TestMultiCapableCarDriver
        {
            StartPosition = start,
            GoalPosition = goal,
            Car = Golf.CreateOnParking(_parkingLayer, _environment, start)
        };
        timeDrivingAgent.Init(layer);
        List<TestMultiCapableCarDriver> agents = new List<TestMultiCapableCarDriver>
        {
            lengthDrivingAgent,
            timeDrivingAgent
        };
        for (int tick = 0; tick < 20000 && !agents.All(agent => agent.GoalReached); tick++)
        {
            foreach (TestMultiCapableCarDriver agent in agents) agent.Tick();
            simulationContext.UpdateStep();
        } // TODO compare both; see cyclist vs pedestrian

        Assert.True(agents.All(agent => agent.GoalReached), start + "," + goal);
        Assert.True(agents.All(agent => agent.HasUsedCar), start + "," + goal);
        Assert.All(agents, agent =>
        {
            Assert.Equal(goal.Longitude, agent.Position.Longitude, 1);
            Assert.Equal(goal.Latitude, agent.Position.Latitude, 1);
        });
    }

    [Fact]
    public void HappyPathWalkingToCarAndDriveToGoalMultimodal()
    {
        TestMultimodalLayer multimodalLayer = new TestMultimodalLayer(_environment);
        ISimulationContext simulationContext = multimodalLayer.Context;

        (Position start, Position goal) = FindReasonableDrivingTour();

        TestMultiCapableCarDriver driver = new TestMultiCapableCarDriver
        {
            StartPosition = start,
            GoalPosition = goal,
            Car = Golf.CreateOnParking(_parkingLayer, _environment, start)
        };
        driver.Init(multimodalLayer);
        TestWalkingPedestrian pedestrian = new TestWalkingPedestrian
        {
            StartPosition = start,
            GoalPosition = goal
        };
        pedestrian.Init(multimodalLayer);
        List<MultimodalAgent<TestMultimodalLayer>> agents = new List<MultimodalAgent<TestMultimodalLayer>>
        {
            driver,
            pedestrian
        };

        for (int tick = 0; tick < 10000 && !agents.All(agent => agent.GoalReached); tick++)
        {
            foreach (MultimodalAgent<TestMultimodalLayer> agent in agents) agent.Tick();
            simulationContext.UpdateStep();
        }

        Assert.Equal(goal.Longitude, driver.Position.Longitude, 0);
        Assert.Equal(goal.Latitude, driver.Position.Latitude, 0);
    }

    [Fact]
    public void ParkingSpotIsGoalSoNoMoreWalkingFromThatPosition()
    {
        TestMultimodalLayer layer = new TestMultimodalLayer(_environment);
        ISimulationContext simulationContext = layer.Context;

        Position? start = Position.CreatePosition(9.9423957, 53.5451441);
        Position? goal = Position.CreatePosition(9.9457685, 53.5454056);

        TestMultiCapableCarDriver driver = new TestMultiCapableCarDriver
        {
            StartPosition = start,
            GoalPosition = goal,
            Car = Golf.CreateOnParking(_parkingLayer, _environment, start)
        };
        driver.Init(layer);
        for (int tick = 0; tick < 3000 && !driver.GoalReached; tick++, simulationContext.UpdateStep())
            driver.Tick();

        Assert.True(driver.GoalReached);
        Assert.True(driver.HasUsedCar);
        Assert.Equal(goal.Longitude, driver.Position.Longitude, 2);
        Assert.Equal(goal.Latitude, driver.Position.Latitude, 2);
    }
}