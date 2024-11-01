using System;
using System.Collections.Generic;
using System.Linq;
using Mars.Components.Environments;
using Mars.Interfaces.Environments;
using Mars.Interfaces.Model;
using Mars.Interfaces.Model.Options;
using SOHModel.Car.Parking;
using SOHModel.Domain.Graph;
using SOHModel.Multimodal.Multimodal;
using SOHModel.Multimodal.Routing;
using SOHTests.Commons.Agent;
using SOHTests.Commons.Environment;
using SOHTests.Commons.Layer;
using Xunit;

namespace SOHTests.MultimodalModelTests.RoutingTests;

public class MultimodalRouteFinderTests
{
    private readonly MultimodalAgent<TestMultimodalLayer> _agent;

    private readonly MultimodalRouteFinder _multimodalRouteFinder;
    private readonly Position _start, _goal;

    public MultimodalRouteFinderTests()
    {
        FourNodeGraphEnv fourNodeGraphEnv = new FourNodeGraphEnv();
        StreetLayer streetLayer = new StreetLayer { Environment = fourNodeGraphEnv.GraphEnvironment };

        TestMultimodalLayer layer = new TestMultimodalLayer(fourNodeGraphEnv.GraphEnvironment)
        {
            BicycleRentalLayer = new FourNodeBicycleRentalLayerFixture(streetLayer).BicycleRentalLayer,
            CarParkingLayer = new FourNodeCarParkingLayerFixture(streetLayer).CarParkingLayer
        };
        _multimodalRouteFinder = new MultimodalRouteFinder(layer.SpatialGraphMediatorLayer);

        _start = FourNodeGraphEnv.Node1Pos;
        _goal = FourNodeGraphEnv.Node4Pos;
        _agent = new TestMultiCapableAgent
        {
            StartPosition = _start,
            CapabilityDrivingOwnCar = true,
            CapabilityCycling = true
        };
        _agent.Init(layer);
    }

    [Fact]
    public void AlwaysFavorDriving()
    {
        List<ModalChoice> capabilities = new List<ModalChoice> { ModalChoice.CarDriving };
        MultimodalRoute route = _multimodalRouteFinder.Search(_agent, _start, _goal, capabilities);
        Assert.Equal(_start, route.Start);
        Assert.Equal(_goal, route.Goal);

        Assert.Equal(3, route.Count);
        Assert.Equal(ModalChoice.Walking, route[0].ModalChoice);
        Assert.Equal(ModalChoice.CarDriving, route[1].ModalChoice);
        Assert.Equal(ModalChoice.Walking, route[2].ModalChoice);

        capabilities = new List<ModalChoice>
            { ModalChoice.CarDriving, ModalChoice.Walking };
        route = _multimodalRouteFinder.Search(_agent, _start, _goal, capabilities);
        Assert.Equal(_start, route.Start);
        Assert.Equal(_goal, route.Goal);

        Assert.Equal(3, route.Count);
        Assert.Equal(ModalChoice.Walking, route[0].ModalChoice);
        Assert.Equal(ModalChoice.CarDriving, route[1].ModalChoice);
        Assert.Equal(ModalChoice.Walking, route[2].ModalChoice);

        capabilities = new List<ModalChoice>
            { ModalChoice.CarDriving, ModalChoice.Walking, ModalChoice.CyclingRentalBike };
        route = _multimodalRouteFinder.Search(_agent, _start, _goal, capabilities);
        Assert.Equal(_start, route.Start);
        Assert.Equal(_goal, route.Goal);

        Assert.Equal(3, route.Count);
        Assert.Equal(ModalChoice.Walking, route[0].ModalChoice);
        Assert.Equal(ModalChoice.CarDriving, route[1].ModalChoice);
        Assert.Equal(ModalChoice.Walking, route[2].ModalChoice);
    }

    [Fact]
    public void FavorCyclingOverWalking()
    {
        List<ModalChoice> capabilities = new List<ModalChoice>
            { ModalChoice.CyclingRentalBike };
        MultimodalRoute route = _multimodalRouteFinder.Search(_agent, _start, _goal, capabilities);
        Assert.Equal(_start, route.Start);
        Assert.Equal(_goal, route.Goal);

        Assert.Equal(3, route.Count);
        Assert.Equal(ModalChoice.Walking, route[0].ModalChoice);
        Assert.Equal(ModalChoice.CyclingRentalBike, route[1].ModalChoice);
        Assert.Equal(ModalChoice.Walking, route[2].ModalChoice);

        capabilities = new List<ModalChoice>
            { ModalChoice.Walking, ModalChoice.CyclingRentalBike };
        route = _multimodalRouteFinder.Search(_agent, _start, _goal, capabilities);
        Assert.Equal(_start, route.Start);
        Assert.Equal(_goal, route.Goal);

        Assert.Equal(3, route.Count);
        Assert.Equal(ModalChoice.Walking, route[0].ModalChoice);
        Assert.Equal(ModalChoice.CyclingRentalBike, route[1].ModalChoice);
        Assert.Equal(ModalChoice.Walking, route[2].ModalChoice);
    }

    [Fact]
    public void Walking()
    {
        MultimodalRoute route = _multimodalRouteFinder.Search(_agent, _start, _goal, ModalChoice.Walking);
        Assert.Equal(_start, route.Start);
        Assert.Equal(_goal, route.Goal);

        Assert.Equal(1, route.Count);
        Assert.Equal(ModalChoice.Walking, route.First().ModalChoice);
    }

    [Fact]
    public void WalkingAndCycling()
    {
        MultimodalRoute route = _multimodalRouteFinder.Search(_agent, _start, _goal, ModalChoice.CyclingRentalBike);
        Assert.Equal(_start, route.Start);
        Assert.Equal(_goal, route.Goal);

        Assert.Equal(3, route.Count);
        Assert.Equal(ModalChoice.Walking, route[0].ModalChoice);
        Assert.Equal(ModalChoice.CyclingRentalBike, route[1].ModalChoice);
        Assert.Equal(ModalChoice.Walking, route[2].ModalChoice);
    }

    [Fact]
    public void WalkingAndDriving()
    {
        MultimodalRoute route = _multimodalRouteFinder.Search(_agent, _start, _goal, ModalChoice.CarDriving);
        Assert.Equal(_start, route.Start);
        Assert.Equal(_goal, route.Goal);

        Assert.Equal(3, route.Count);
        Assert.Equal(ModalChoice.Walking, route[0].ModalChoice);
        Assert.Equal(ModalChoice.CarDriving, route[1].ModalChoice);
        Assert.Equal(ModalChoice.Walking, route[2].ModalChoice);
    }

    [Fact]
    public void FindWalkingDrivingMultimodalRouteButMissingWalkGraph()
    {
        SpatialGraphEnvironment environment = new SpatialGraphEnvironment(new SpatialGraphOptions
        {
            GraphImports = new List<Input>
            {
                // accidentally not added to the graph
                // new()
                // {
                //     File = ResourcesConstants.WalkGraphAltonaAltstadt,
                //     InputConfiguration = new InputConfiguration
                //         { Modalities = new HashSet<SpatialModalityType> { SpatialModalityType.Walking } }
                // },
                new()
                {
                    File = ResourcesConstants.DriveGraphAltonaAltstadt,
                    InputConfiguration = new InputConfiguration
                        { Modalities = new HashSet<SpatialModalityType> { SpatialModalityType.CarDriving } }
                }
            }
        });

        CarParkingLayer parking = new CarParkingLayerFixture(new StreetLayer { Environment = environment }).CarParkingLayer;
        TestMultimodalLayer layer = new TestMultimodalLayer(environment) { CarParkingLayer = parking };
        TestMultiCapableAgent agent = new TestMultiCapableAgent { StartPosition = Position.CreatePosition(9.9517071, 53.5575623) };
        agent.Init(layer);

        Position? goal = Position.CreatePosition(9.9517643, 53.5517866);
        Assert.Throws<ApplicationException>(() =>
            layer.Search(_agent, agent.StartPosition, goal, ModalChoice.CarDriving));
    }

    [Fact]
    public void FindWalkingDrivingMultimodalRouteButMissingDrivingGraph()
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
                }
                // accidentally not added to the graph
                // new()
                // {
                //     File = ResourcesConstants.DriveGraphAltonaAltstadt, 
                //     InputConfiguration = new InputConfiguration { Modalities = new HashSet<SpatialModalityType> { SpatialModalityType.CarDriving } }
                // }
            }
        });

        CarParkingLayer parking = new CarParkingLayerFixture(new StreetLayer { Environment = environment }).CarParkingLayer;
        TestMultimodalLayer layer = new TestMultimodalLayer(environment) { CarParkingLayer = parking };
        TestMultiCapableAgent agent = new TestMultiCapableAgent { StartPosition = Position.CreatePosition(9.9517071, 53.5575623) };
        agent.Init(layer);

        Position? goal = Position.CreatePosition(9.9517643, 53.5517866);
        Assert.Throws<ApplicationException>(() =>
            layer.Search(_agent, agent.StartPosition, goal, ModalChoice.CarDriving));
    }
}