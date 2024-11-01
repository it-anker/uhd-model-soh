using System.Linq;
using Mars.Interfaces;
using Mars.Interfaces.Agents;
using Mars.Interfaces.Environments;
using Mars.Interfaces.Layers;
using SOHModel.Car.Model;
using SOHTests.Commons.Environment;
using SOHTests.Commons.Layer;
using Xunit;

namespace SOHTests.CarModelTests;

public class CarDriverTests
{
    private static void Register(ILayer layer, ITickClient tickClient)
    {
        // do nothing
    }

    private static void Unregister(ILayer layer, ITickClient tickClient)
    {
        // do nothing
    }

    [Fact]
    public void DriveSimpleRoute()
    {
        SimulationContext? simulationContext = SimulationContext.Start2020InSeconds;
        FourNodeGraphEnv environment = new FourNodeGraphEnv();

        CarLayer carLayer = new CarLayerFixture(environment.GraphEnvironment).CarLayer;

        Position start = FourNodeGraphEnv.Node1Pos;
        Position goal = FourNodeGraphEnv.Node4Pos;

        CarDriver driver = new CarDriver(carLayer, Register, Unregister, 3,
            start.Latitude, start.Longitude, goal.Latitude, goal.Longitude);

        Assert.Equal(start, driver.Position);

        Position lastDriverPosition = driver.Position;
        for (int tick = 0; tick < 1000 && !driver.GoalReached; tick++, simulationContext.UpdateStep())
        {
            driver.Tick();

            Assert.NotEqual(lastDriverPosition, driver.Position);
            lastDriverPosition = driver.Position;
        }

        Assert.True(driver.GoalReached);
        Assert.InRange(driver.Position.DistanceInMTo(goal), 0, 10);
    }

    [Fact]
    public void InitCarDriverAndMoveFirstTick()
    {
        const double standardSpeedLimit = 13.89;

        FourNodeGraphEnv environment = new FourNodeGraphEnv();
        CarLayer carLayer = new CarLayerFixture(environment.GraphEnvironment).CarLayer;

        Position start = FourNodeGraphEnv.Node1Pos;
        Position goal = FourNodeGraphEnv.Node4Pos;

        CarDriver driver = new CarDriver(carLayer, Register, Unregister, 3, start.Latitude, start.Longitude,
            goal.Latitude, goal.Longitude);

        Assert.Equal(FourNodeGraphEnv.Node1Pos.Latitude, driver.Latitude);
        Assert.Equal(FourNodeGraphEnv.Node1Pos.Longitude, driver.Longitude);
        Assert.Equal(0, driver.Velocity);
        Assert.Equal(standardSpeedLimit, driver.MaxSpeed, 2);
        Assert.Null(driver.StableId);
        Assert.True(driver.RemainingRouteDistanceToGoal > 1);
        Assert.False(driver.GoalReached);

        driver.Tick();

        Assert.False(driver.GoalReached);
        Assert.True(driver.Velocity > 0);
        ISpatialEdge? edge = environment.Node1.OutgoingEdges.Values.FirstOrDefault();
        Assert.NotNull(edge);
        Assert.Equal(edge!.Length - driver.Velocity, driver.RemainingDistanceOnEdge, 1);
        Assert.Equal(edge.Length - driver.RemainingDistanceOnEdge, driver.PositionOnEdge, 2);
        Assert.Equal("-1", driver.CurrentEdgeId);
    }
}