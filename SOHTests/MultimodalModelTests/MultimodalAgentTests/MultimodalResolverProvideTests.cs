using System.Collections.Generic;
using System.Linq;
using Mars.Components.Environments;
using Mars.Interfaces.Environments;
using Mars.Interfaces.Model;
using Mars.Interfaces.Model.Options;
using SOHModel.Bicycle.Model;
using SOHModel.Bicycle.Parking;
using SOHModel.Bicycle.Rental;
using SOHModel.Car.Model;
using SOHModel.Car.Parking;
using SOHModel.Domain.Graph;
using SOHTests.Commons.Agent;
using SOHTests.Commons.Layer;
using Xunit;

namespace SOHTests.MultimodalModelTests.MultimodalAgentTests;

public class MultimodalResolverProvideTests
{
    private readonly SpatialGraphEnvironment _environment;
    private readonly TestMultimodalLayer _multimodalLayer;

    public MultimodalResolverProvideTests()
    {
        SpatialGraphOptions options = new SpatialGraphOptions
        {
            GraphImports = new List<Input>
            {
                new()
                {
                    File = ResourcesConstants.DriveGraphAltonaAltstadt,
                    InputConfiguration = new InputConfiguration
                    {
                        Modalities = new HashSet<SpatialModalityType> { SpatialModalityType.Cycling }
                    }
                },
                new()
                {
                    File = ResourcesConstants.WalkGraphAltonaAltstadt,
                    InputConfiguration = new InputConfiguration
                    {
                        Modalities = new HashSet<SpatialModalityType> { SpatialModalityType.Walking }
                    }
                },
                new()
                {
                    File = ResourcesConstants.DriveGraphAltonaAltstadt,
                    InputConfiguration = new InputConfiguration
                    {
                        Modalities = new HashSet<SpatialModalityType> { SpatialModalityType.CarDriving }
                    }
                }
            }
        };

        _environment = new SpatialGraphEnvironment(options);

        BicycleRentalLayer bicycleRentalLayer = new BicycleRentalLayerFixture(_environment).BicycleRentalLayer;
        CarParkingLayer carParkingLayer = new CarParkingLayerFixture(new StreetLayer { Environment = _environment })
            .CarParkingLayer;
        BicycleParkingLayer bicycleParkingLayer = new BicycleParkingLayerFixture(_environment).BicycleParkingLayer;

        _multimodalLayer = new TestMultimodalLayer(_environment)
        {
            CarParkingLayer = carParkingLayer,
            BicycleRentalLayer = bicycleRentalLayer,
            BicycleParkingLayer = bicycleParkingLayer
        };
    }

    [Fact]
    public void TestProvidesForWalking()
    {
        TestCapabilitiesAgent agent = new TestCapabilitiesAgent(ModalChoice.Walking);
        foreach (ISpatialNode node in _environment.Nodes)
            Assert.Contains(ModalChoice.Walking, _multimodalLayer.Provides(agent, node));
    }

    [Fact]
    public void TestProvidesForRentalCycling()
    {
        TestCapabilitiesAgent agent = new TestCapabilitiesAgent(ModalChoice.CyclingRentalBike);
        HashSet<ISpatialNode> foundNodes = new HashSet<ISpatialNode>();

        foreach (BicycleRentalStation rentalStation in _multimodalLayer.BicycleRentalLayer.Features.OfType<BicycleRentalStation>())
        {
            ISpatialNode nearestNode = _environment.NearestNode(rentalStation.Position);
            if (!_multimodalLayer.Provides(agent, nearestNode).Contains(ModalChoice.CyclingRentalBike))
                _multimodalLayer.Provides(agent, nearestNode);

            Assert.Contains(ModalChoice.CyclingRentalBike, _multimodalLayer.Provides(agent, nearestNode));
            foundNodes.Add(nearestNode);
        }

        foreach (ISpatialNode node in _environment.Nodes)
            if (!foundNodes.Contains(node))
                Assert.DoesNotContain(ModalChoice.CyclingRentalBike, _multimodalLayer.Provides(agent, node));
    }

    [Fact]
    public void TestProvidesForCyclingOwnBike()
    {
        Position? start = Position.CreateGeoPosition(9.9546178, 53.557155);
        Bicycle bicycle = _multimodalLayer.BicycleParkingLayer.CreateOwnBicycleNear(start, -1, 0f);
        TestCapabilitiesAgent agent = new TestCapabilitiesAgent(ModalChoice.CyclingOwnBike)
        {
            Bicycle = bicycle
        };

        ISpatialNode bicycleNode = _environment.NearestNode(bicycle.Position);
        Assert.Contains(ModalChoice.CyclingOwnBike, _multimodalLayer.Provides(agent, bicycleNode));

        foreach (ISpatialNode node in _environment.Nodes)
            if (!bicycleNode.Equals(node))
                Assert.DoesNotContain(ModalChoice.CyclingOwnBike, _multimodalLayer.Provides(agent, node));
    }

    [Fact]
    public void TestProvidesForDriving()
    {
        Position? start = Position.CreateGeoPosition(9.9546178, 53.557155);
        Car car = _multimodalLayer.CarParkingLayer.CreateOwnCarNear(start);
        TestCapabilitiesAgent agent = new TestCapabilitiesAgent(ModalChoice.CarDriving)
        {
            Car = car
        };

        ISpatialNode carNode = _environment.NearestNode(start);
        Assert.Contains(ModalChoice.CarDriving, _multimodalLayer.Provides(agent, carNode));

        foreach (ISpatialNode node in _environment.Nodes)
            if (!carNode.Equals(node))
                Assert.DoesNotContain(ModalChoice.CarDriving, _multimodalLayer.Provides(agent, node));
    }
}