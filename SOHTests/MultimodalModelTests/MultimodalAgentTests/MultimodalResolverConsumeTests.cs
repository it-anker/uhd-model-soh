using System.Collections.Generic;
using System.Linq;
using Mars.Components.Environments;
using Mars.Interfaces.Environments;
using Mars.Interfaces.Model;
using Mars.Interfaces.Model.Options;
using SOHModel.Bicycle.Rental;
using SOHModel.Car.Model;
using SOHModel.Car.Parking;
using SOHModel.Domain.Graph;
using SOHTests.Commons.Layer;
using Xunit;

namespace SOHTests.MultimodalModelTests.MultimodalAgentTests;

public class MultimodalResolverConsumeTests
{
    private readonly SpatialGraphEnvironment _environment;
    private readonly TestMultimodalLayer _multimodalLayer;

    public MultimodalResolverConsumeTests()
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

        _multimodalLayer = new TestMultimodalLayer(_environment)
        {
            CarParkingLayer = carParkingLayer,
            BicycleRentalLayer = bicycleRentalLayer
        };
    }

    [Fact]
    public void TestConsumeForWalking()
    {
        foreach (ISpatialNode node in _environment.Nodes)
            Assert.True(_multimodalLayer.Consumes(ModalChoice.Walking, node));
    }

    [Fact]
    public void TestConsumesForRentalCycling()
    {
        HashSet<ISpatialNode> foundNodes = new HashSet<ISpatialNode>();
        foreach (BicycleRentalStation rentalStation in _multimodalLayer.BicycleRentalLayer.Features.OfType<BicycleRentalStation>())
        {
            ISpatialNode nearestNode = _environment.NearestNode(rentalStation.Position);

            Assert.True(_multimodalLayer.Consumes(ModalChoice.CyclingRentalBike, nearestNode));
            foundNodes.Add(nearestNode);
        }

        foreach (ISpatialNode node in _environment.Nodes)
            if (!foundNodes.Contains(node))
                Assert.False(_multimodalLayer.Consumes(ModalChoice.CyclingRentalBike, node));
    }

    [Fact]
    public void TestConsumesForCyclingOwnBike()
    {
        foreach (ISpatialNode node in _environment.Nodes)
            Assert.True(_multimodalLayer.Consumes(ModalChoice.CyclingOwnBike, node));
    }

    [Fact]
    public void TestConsumesForDriving()
    {
        CarParkingSpace? carParkingSpace = _multimodalLayer.CarParkingLayer.Nearest(
                Position.CreateGeoPosition(9.9528571, 53.5505072));
        ISpatialNode nearestNode = _environment.NearestNode(carParkingSpace.Position);
        
        Assert.False(carParkingSpace.Occupied);
        Assert.True(_multimodalLayer.Consumes(ModalChoice.CarDriving, nearestNode));

        carParkingSpace.Occupied = true;
        Assert.False(_multimodalLayer.Consumes(ModalChoice.CarDriving, nearestNode));

        carParkingSpace.Occupied = false;
        Assert.True(_multimodalLayer.Consumes(ModalChoice.CarDriving, nearestNode));

        for (int i = 0; i < carParkingSpace.Capacity; i++) Assert.True(carParkingSpace.Enter(new Car()));
        Assert.False(_multimodalLayer.Consumes(ModalChoice.CarDriving, nearestNode));
    }
}