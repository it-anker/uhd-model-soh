using System.Linq;
using Mars.Components.Environments;
using Mars.Interfaces;
using SOHTests.Commons.Agent;
using Xunit;

namespace SOHTests.CarModelTests.BrakingTests;

public class BrakingTests
{
    [Fact]
    public void BrakingFrom30Kmh()
    {
        const double speed = 30 / 3.6;

        SpatialGraphEnvironment graph = new SpatialGraphEnvironment(ResourcesConstants.RingNetwork);
        graph.Edges.Values.First().MaxSpeed = speed; // set speed limit

        SimulationContext? context = SimulationContext.Start2020InSeconds;
        InfiniteSteeringDriver driver = new InfiniteSteeringDriver(context, 0, graph, 0, speed)
        {
            // set start speed
            Car = { Velocity = speed }
        };

        Assert.False(driver.BrakingActivated);

        const int tickToBrake = 10;
        int tickWhenStopped = -1;
        double distanceBeforeBrake = -1d;

        for (int tick = 0; tick < 50; tick++, context.UpdateStep())
        {
            double velocityLastTick = driver.Velocity;
            driver.Tick();

            switch (tick)
            {
                case tickToBrake:
                    driver.BrakingActivated = true;
                    distanceBeforeBrake = driver.PositionOnCurrentEdge;
                    break;
                case > tickToBrake:
                    Assert.True(driver.BrakingActivated);
                    Assert.True(driver.Velocity <= velocityLastTick);

                    if (tickWhenStopped < 0 && driver.Velocity == 0) tickWhenStopped = tick;
                    break;
            }
        }

        Assert.Equal(0.0, driver.Velocity);

        int brakingTime = tickWhenStopped - tickToBrake;
        Assert.Equal(2, brakingTime);

        double brakingDistance = driver.PositionOnCurrentEdge - distanceBeforeBrake;
        Assert.InRange(brakingDistance, 3, 4);
    }

    [Fact]
    public void BrakingFrom50Kmh()
    {
        const double speed = 50 / 3.6;

        SpatialGraphEnvironment graph = new SpatialGraphEnvironment(ResourcesConstants.RingNetwork);
        graph.Edges.Values.First().MaxSpeed = speed; // set speed limit

        SimulationContext? context = SimulationContext.Start2020InSeconds;
        InfiniteSteeringDriver driver = new InfiniteSteeringDriver(context, 0, graph, 0, speed)
        {
            // set start speed
            Car =
            {
                Velocity = speed
            }
        };

        Assert.False(driver.BrakingActivated);

        const int tickToBrake = 10;
        int tickWhenStopped = -1;
        double distanceBeforeBrake = -1d;

        for (int tick = 0; tick < 50; tick++, context.UpdateStep())
        {
            double velocityLastTick = driver.Velocity;
            driver.Tick();

            switch (tick)
            {
                case tickToBrake:
                    driver.BrakingActivated = true;
                    distanceBeforeBrake = driver.PositionOnCurrentEdge;
                    break;
                case > tickToBrake:
                    Assert.True(driver.BrakingActivated);
                    Assert.True(driver.Velocity <= velocityLastTick);

                    if (tickWhenStopped < 0 && driver.Velocity == 0) tickWhenStopped = tick;
                    break;
            }
        }

        Assert.Equal(0.0, driver.Velocity);

        int brakingTime = tickWhenStopped - tickToBrake;
        Assert.Equal(4, brakingTime);

        double brakingDistance = driver.PositionOnCurrentEdge - distanceBeforeBrake;
        Assert.InRange(brakingDistance, 17, 18);
    }

    [Fact]
    public void BrakingFrom70Kmh()
    {
        const double speed = 70 / 3.6;

        SpatialGraphEnvironment graph = new SpatialGraphEnvironment(ResourcesConstants.RingNetwork);
        graph.Edges.Values.First().MaxSpeed = speed; // set speed limit

        SimulationContext? context = SimulationContext.Start2020InSeconds;
        InfiniteSteeringDriver driver = new InfiniteSteeringDriver(context, 0, graph, 0, speed)
        {
            // set start speed
            Car =
            {
                Velocity = speed
            }
        };

        Assert.False(driver.BrakingActivated);

        const int tickToBrake = 10;
        int tickWhenStopped = -1;
        double distanceBeforeBrake = -1d;

        for (int tick = 0; tick < 50; tick++, context.UpdateStep())
        {
            double velocityLastTick = driver.Velocity;
            driver.Tick();

            switch (tick)
            {
                case tickToBrake:
                    driver.BrakingActivated = true;
                    distanceBeforeBrake = driver.PositionOnCurrentEdge;
                    break;
                case > tickToBrake:
                    Assert.True(driver.BrakingActivated);
                    Assert.True(driver.Velocity <= velocityLastTick);

                    if (tickWhenStopped < 0 && driver.Velocity == 0) tickWhenStopped = tick;
                    break;
            }
        }

        Assert.Equal(0.0, driver.Velocity);

        int brakingTime = tickWhenStopped - tickToBrake;
        Assert.Equal(6, brakingTime);

        double brakingDistance = driver.PositionOnCurrentEdge - distanceBeforeBrake;
        Assert.InRange(brakingDistance, 47, 48);
    }

    [Fact]
    public void BrakingFrom100Kmh()
    {
        const double speed = 100 / 3.6;

        SpatialGraphEnvironment graph = new SpatialGraphEnvironment(ResourcesConstants.RingNetwork);
        graph.Edges.Values.First().MaxSpeed = speed; // set speed limit

        SimulationContext? context = SimulationContext.Start2020InSeconds;
        InfiniteSteeringDriver driver = new InfiniteSteeringDriver(context, 0, graph, 0, speed)
        {
            // set start speed
            Car =
            {
                Velocity = speed
            }
        };

        Assert.False(driver.BrakingActivated);

        const int tickToBrake = 10;
        int tickWhenStopped = -1;
        double distanceBeforeBrake = -1d;

        for (int tick = 0; tick < 50; tick++, context.UpdateStep())
        {
            double velocityLastTick = driver.Velocity;
            driver.Tick();

            switch (tick)
            {
                case tickToBrake:
                    driver.BrakingActivated = true;
                    distanceBeforeBrake = driver.PositionOnCurrentEdge;
                    break;
                case > tickToBrake:
                    Assert.True(driver.BrakingActivated);
                    Assert.True(driver.Velocity <= velocityLastTick);

                    if (tickWhenStopped < 0 && driver.Velocity == 0) tickWhenStopped = tick;
                    break;
            }
        }

        Assert.Equal(0.0, driver.Velocity);

        int brakingTime = tickWhenStopped - tickToBrake;
        Assert.Equal(9, brakingTime);

        double brakingDistance = driver.PositionOnCurrentEdge - distanceBeforeBrake;
        Assert.InRange(brakingDistance, 121, 122);
    }
}