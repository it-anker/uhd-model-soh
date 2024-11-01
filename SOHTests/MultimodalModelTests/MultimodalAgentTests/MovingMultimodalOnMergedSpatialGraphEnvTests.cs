using System.Collections.Generic;
using Mars.Components.Environments;
using Mars.Interfaces.Environments;
using Mars.Interfaces.Layers;
using Mars.Interfaces.Model;
using Mars.Interfaces.Model.Options;
using NetTopologySuite.Geometries;
using SOHModel.Bicycle.Rental;
using SOHModel.Car.Model;
using SOHModel.Car.Parking;
using SOHModel.Domain.Graph;
using SOHModel.Multimodal.Multimodal;
using SOHTests.Commons.Agent;
using SOHTests.Commons.Layer;
using Xunit;
using Position = Mars.Interfaces.Environments.Position;

namespace SOHTests.MultimodalModelTests.MultimodalAgentTests;

public class MovingMultimodalOnMergedSpatialGraphEnvTests
{
    private readonly SpatialGraphOptions _options;

    public MovingMultimodalOnMergedSpatialGraphEnvTests()
    {
        _options = new SpatialGraphOptions
        {
            // NetworkMerge = true,
            GraphImports = new List<Input>
            {
                new()
                {
                    File = ResourcesConstants.DriveGraphAltonaAltstadt,
                    InputConfiguration = new InputConfiguration
                    {
                        Modalities = new HashSet<SpatialModalityType> { SpatialModalityType.Cycling },
                        IsBiDirectedImport = true
                    }
                },
                new()
                {
                    File = ResourcesConstants.WalkGraphAltonaAltstadt,
                    InputConfiguration = new InputConfiguration
                    {
                        Modalities = new HashSet<SpatialModalityType> { SpatialModalityType.Walking },
                        IsBiDirectedImport = true
                    }
                },
                new()
                {
                    File = ResourcesConstants.DriveGraphAltonaAltstadt,
                    InputConfiguration = new InputConfiguration
                    {
                        Modalities = new HashSet<SpatialModalityType> { SpatialModalityType.CarDriving },
                        IsBiDirectedImport = true
                    }
                }
            }
        };
    }

    [Fact]
    public void AllRentalStationNodesCanReachEachOther()
    {
        SpatialGraphEnvironment environment = new SpatialGraphEnvironment(_options);
        BicycleRentalLayer bicycleRentalLayer = new BicycleRentalLayerFixture(environment).BicycleRentalLayer;

        foreach (IVectorFeature start in bicycleRentalLayer.Features)
        foreach (IVectorFeature target in bicycleRentalLayer.Features)
        {
            if (start == target) continue;

            Point? startPoint = start.VectorStructured.Geometry.Centroid;
            ISpatialNode startNode = environment.NearestNode(Position.CreateGeoPosition(startPoint.X, startPoint.Y));
            Point? targetPoint = target.VectorStructured.Geometry.Centroid;
            ISpatialNode targetNode = environment.NearestNode(Position.CreateGeoPosition(targetPoint.X, targetPoint.Y));
            Route route = environment.FindRoute(startNode, targetNode);
            Assert.NotNull(route);
        }
    }

    [Fact]
    public void DriveOnDrivingLane()
    {
        SpatialGraphEnvironment environment = new SpatialGraphEnvironment(_options);
        BicycleRentalLayer bicycleRentalLayer = new BicycleRentalLayerFixture(environment).BicycleRentalLayer;
        CarParkingLayer carParkingLayer = new CarParkingLayerFixture(new StreetLayer { Environment = environment })
            .CarParkingLayer;

        Position? start = Position.CreateGeoPosition(9.9546178, 53.557155);
        Position? goal = Position.CreateGeoPosition(9.9418041, 53.5480482);

        Car car = carParkingLayer.CreateOwnCarNear(start);

        TestMultimodalLayer layer = new TestMultimodalLayer(environment)
        {
            CarParkingLayer = carParkingLayer,
            BicycleRentalLayer = bicycleRentalLayer
        };
        TestMultiCapableAgent agent = new TestMultiCapableAgent
        {
            StartPosition = start,
            GoalPosition = goal,
            ModalChoice = ModalChoice.CarDriving,
            CarParkingLayer = carParkingLayer,
            BicycleRentalLayer = bicycleRentalLayer,
            Car = car
        };
        agent.Init(layer);

        Assert.Equal(Whereabouts.Offside, agent.Whereabouts);
        Assert.Equal(start, agent.Position);
        Assert.Equal(ModalChoice.CarDriving, agent.RouteMainModalChoice);

        Assert.NotEmpty(agent.MultimodalRoute);
        Assert.True(agent.MultimodalRoute.RouteLength > 0);

        Assert.False(agent.GoalReached);
        const int ticks = 5000;
        for (int tick = 0; tick < ticks && !agent.GoalReached; tick++)
        {
            agent.Tick();

            ISpatialEdge? edge = agent.Car.CurrentEdge;
            if (edge?.Modalities.Contains(SpatialModalityType.CarDriving) ?? false)
            {
                (int minLane, int maxLane) = edge.ModalityLaneRanges[SpatialModalityType.CarDriving];
                if (agent.CurrentlyCarDriving)
                    Assert.InRange(agent.Car.LaneOnCurrentEdge, minLane, maxLane);
                else
                    Assert.NotInRange(agent.Car.LaneOnCurrentEdge, minLane, maxLane);
            }
        }

        agent.Tick();
        Assert.True(agent.GoalReached);

        Assert.Equal(Whereabouts.Offside, agent.Whereabouts);
        Assert.Equal(goal, agent.Position);
    }

    [Fact]
    public void CycleOnCyclingLane()
    {
        SpatialGraphEnvironment environment = new SpatialGraphEnvironment(_options);
        BicycleRentalLayer bicycleRentalLayer = new BicycleRentalLayerFixture(environment).BicycleRentalLayer;

        Position? start = Position.CreateGeoPosition(9.9546178, 53.557155);
        Position? goal = Position.CreateGeoPosition(9.9418041, 53.5480482);

        TestMultimodalLayer layer = new TestMultimodalLayer(environment)
        {
            BicycleRentalLayer = bicycleRentalLayer
        };
        TestMultiCapableAgent agent = new TestMultiCapableAgent
        {
            StartPosition = start,
            GoalPosition = goal,
            ModalChoice = ModalChoice.CyclingRentalBike,
            BicycleRentalLayer = bicycleRentalLayer
        };
        agent.Init(layer);

        Assert.Equal(Whereabouts.Offside, agent.Whereabouts);
        Assert.Equal(start, agent.Position);
        Assert.Equal(ModalChoice.CyclingRentalBike, agent.RouteMainModalChoice);

        MultimodalRoute route = agent.MultimodalRoute;
        Assert.NotEmpty(route);
        Assert.True(route.RouteLength > 0);
        Assert.Equal(3, route.Count);

        Assert.False(agent.GoalReached);
        const int ticks = 5000;

        for (int tick = 0; tick < ticks; tick++)
        {
            agent.Tick();

            if (agent.RentalBicycle == null) continue;

            ISpatialEdge? edge = agent.RentalBicycle.CurrentEdge;
            if (edge?.Modalities.Contains(SpatialModalityType.Cycling) ?? false)
            {
                (int minLane, int maxLane) = edge.ModalityLaneRanges[SpatialModalityType.Cycling];
                Assert.InRange(agent.RentalBicycle.LaneOnCurrentEdge, minLane, maxLane);
            }
        }

        Assert.True(agent.GoalReached);

        Assert.Equal(Whereabouts.Offside, agent.Whereabouts);
        Assert.InRange(goal.DistanceInMTo(agent.Position), 0, 1);
        Assert.Equal(goal, agent.Position);
    }
}