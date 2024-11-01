using Mars.Interfaces;
using Mars.Interfaces.Environments;
using SOHModel.Car.Model;
using SOHModel.Car.Parking;
using SOHModel.Domain.Graph;
using SOHTests.Commons.Agent;
using SOHTests.Commons.Environment;
using SOHTests.Commons.Layer;
using Xunit;

namespace SOHTests.MultimodalModelTests.MultimodalDrivingTests;

public class WalkDriveFourNodeEnvTests
{
    private static Golf CreateCarOnNode2(FourNodeGraphEnv fourNodeGraphEnv)
    {
        StreetLayer streetLayer = new StreetLayer { Environment = fourNodeGraphEnv.GraphEnvironment };
        CarParkingLayer parkingLayer = new FourNodeCarParkingLayerFixture(streetLayer).CarParkingLayer;
        return Golf.CreateOnParking(parkingLayer, fourNodeGraphEnv.GraphEnvironment,
            fourNodeGraphEnv.Node2.Position);
    }

    private static void StartSimulation(TestMultiCapableCarDriver driver, IParkingCar car,
        ISimulationContext contextImpl)
    {
        CarParkingSpace? firstParking = car.CarParkingSpace;

        for (int tick = 0; tick < 1000 && !driver.GoalReached; tick++, contextImpl.UpdateStep()) driver.Tick();

        Assert.NotNull(car.CarParkingSpace);
        Assert.NotEqual(car.CarParkingSpace, firstParking);
    }

    [Fact]
    public void Drive()
    {
        FourNodeGraphEnv fourNodeGraphEnv = new FourNodeGraphEnv();
        TestMultimodalLayer layer = new TestMultimodalLayer(fourNodeGraphEnv.GraphEnvironment);

        Position? start = fourNodeGraphEnv.Node2.Position;
        Position? goal = fourNodeGraphEnv.Node3.Position;

        Golf car = CreateCarOnNode2(fourNodeGraphEnv);
        TestMultiCapableCarDriver driver = new TestMultiCapableCarDriver
        {
            StartPosition = start,
            GoalPosition = goal,
            Car = car
        };
        driver.Init(layer);

        StartSimulation(driver, car, layer.Context);

        Assert.True(driver.HasUsedCar);
        Assert.Equal(goal.Longitude, driver.Position.Longitude, 2);
        Assert.Equal(goal.Latitude, driver.Position.Latitude, 2);
    }

    [Fact]
    public void DriveWalk()
    {
        FourNodeGraphEnv fourNodeGraphEnv = new FourNodeGraphEnv();
        TestMultimodalLayer layer = new TestMultimodalLayer(fourNodeGraphEnv.GraphEnvironment);

        Position? start = fourNodeGraphEnv.Node2.Position;
        Position? goal = fourNodeGraphEnv.Node4.Position;

        Golf car = CreateCarOnNode2(fourNodeGraphEnv);
        TestMultiCapableCarDriver driver = new TestMultiCapableCarDriver
        {
            StartPosition = start,
            GoalPosition = goal,
            Car = car
        };
        driver.Init(layer);

        StartSimulation(driver, car, layer.Context);

        Assert.True(driver.HasUsedCar);
        Assert.Equal(goal.Longitude, driver.Position.Longitude, 2);
        Assert.Equal(goal.Latitude, driver.Position.Latitude, 2);
    }

    [Fact]
    public void StartIsGoal()
    {
        FourNodeGraphEnv fourNodeGraphEnv = new FourNodeGraphEnv();
        TestMultimodalLayer layer = new TestMultimodalLayer(fourNodeGraphEnv.GraphEnvironment);

        Position? start = fourNodeGraphEnv.Node2.Position;
        Position? goal = fourNodeGraphEnv.Node2.Position;

        Golf car = CreateCarOnNode2(fourNodeGraphEnv);
        TestMultiCapableCarDriver driver = new TestMultiCapableCarDriver
        {
            StartPosition = start,
            GoalPosition = goal,
            Car = car
        };
        driver.Init(layer);

        for (int tick = 0; tick < 10000 && !driver.GoalReached; tick++) driver.Tick();

        Assert.Equal(goal.Longitude, driver.Position.Longitude, 2);
        Assert.Equal(goal.Latitude, driver.Position.Latitude, 2);
    }

    [Fact]
    public void WalkDrive()
    {
        FourNodeGraphEnv fourNodeGraphEnv = new FourNodeGraphEnv();
        TestMultimodalLayer layer = new TestMultimodalLayer(fourNodeGraphEnv.GraphEnvironment);

        Position? start = fourNodeGraphEnv.Node1.Position;
        Position? goal = fourNodeGraphEnv.Node3.Position;

        Golf car = CreateCarOnNode2(fourNodeGraphEnv);
        TestMultiCapableCarDriver driver = new TestMultiCapableCarDriver
        {
            StartPosition = start,
            GoalPosition = goal,
            Car = car
        };
        driver.Init(layer);

        StartSimulation(driver, car, layer.Context);

        Assert.True(driver.HasUsedCar);
        Assert.Equal(goal.Longitude, driver.Position.Longitude, 3);
        Assert.Equal(goal.Latitude, driver.Position.Latitude, 3);
    }

    [Fact]
    public void WalkDriveWalk()
    {
        FourNodeGraphEnv fourNodeGraphEnv = new FourNodeGraphEnv();
        TestMultimodalLayer layer = new TestMultimodalLayer(fourNodeGraphEnv.GraphEnvironment);

        Position? start = fourNodeGraphEnv.Node1.Position;
        Position? goal = fourNodeGraphEnv.Node4.Position;

        Golf car = CreateCarOnNode2(fourNodeGraphEnv);
        TestMultiCapableCarDriver driver = new TestMultiCapableCarDriver
        {
            StartPosition = start,
            GoalPosition = goal,
            Car = car
        };
        driver.Init(layer);

        StartSimulation(driver, car, layer.Context);

        Assert.True(driver.HasUsedCar);
        Assert.Equal(goal.Longitude, driver.Position.Longitude, 5);
        Assert.Equal(goal.Latitude, driver.Position.Latitude, 5);
    }
}