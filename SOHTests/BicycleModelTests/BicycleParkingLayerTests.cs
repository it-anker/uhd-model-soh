using System.Collections.Generic;
using System.Linq;
using Mars.Components.Environments;
using Mars.Interfaces.Environments;
using Mars.Interfaces.Model;
using NetTopologySuite.Geometries;
using SOHModel.Bicycle.Model;
using SOHModel.Bicycle.Parking;
using SOHTests.Commons.Layer;
using Xunit;
using Position = Mars.Interfaces.Environments.Position;

namespace SOHTests.BicycleModelTests;

public class BicycleParkingLayerTests
{
    [Fact]
    public void TestCreateOwnBicycleOnNodes()
    {
        BicycleParkingLayer bicycleParkingLayer = CreateBicycleParkingLayer();

        Coordinate? centre = bicycleParkingLayer.Features.First().VectorStructured.Geometry.Coordinate;
        Position? position = Position.CreateGeoPosition(centre.X, centre.Y);

        Bicycle bicycle = bicycleParkingLayer.CreateOwnBicycleNear(position, 20, 0);
        Assert.NotNull(bicycle);
        Assert.Null(bicycle.BicycleParkingLot);
        Assert.InRange(bicycle.Position.DistanceInMTo(position), 0, 20);
        Assert.NotNull(bicycle.Environment);
        Assert.Contains(bicycle, bicycle.Environment.Entities.Keys);
    }

    [Fact]
    public void TestCreateOwnBicycleInParkingLotWithRadius()
    {
        BicycleParkingLayer bicycleParkingLayer = CreateBicycleParkingLayer();

        Coordinate? centre = bicycleParkingLayer.Features.First().VectorStructured.Geometry.Coordinate;
        Position? position = Position.CreateGeoPosition(centre.X, centre.Y);

        Bicycle bicycle = bicycleParkingLayer.CreateOwnBicycleNear(position, 20, 1);
        Assert.NotNull(bicycle);
        Assert.NotNull(bicycle.BicycleParkingLot);
        Assert.InRange(bicycle.Position.DistanceInMTo(position), 0, 20);
        Assert.NotNull(bicycle.Environment);
        Assert.Contains(bicycle, bicycle.Environment.Entities.Keys);

        Bicycle bicycle2 = bicycleParkingLayer.CreateOwnBicycleNear(position, 50, 1);
        Assert.NotNull(bicycle2);
        Assert.NotNull(bicycle2.BicycleParkingLot);
        Assert.InRange(bicycle2.Position.DistanceInMTo(position), 0, 50);
        Assert.NotNull(bicycle2.Environment);
        Assert.Contains(bicycle2, bicycle2.Environment.Entities.Keys);
    }

    private static BicycleParkingLayer CreateBicycleParkingLayer()
    {
        SpatialGraphEnvironment environment = new SpatialGraphEnvironment(new Input
        {
            File = ResourcesConstants.DriveGraphAltonaAltstadt,
            InputConfiguration = new InputConfiguration
            {
                IsBiDirectedImport = true,
                Modalities = new HashSet<SpatialModalityType> { SpatialModalityType.Cycling }
            }
        });

        BicycleParkingLayer bicycleParkingLayer = new BicycleParkingLayerFixture(environment).BicycleParkingLayer;
        return bicycleParkingLayer;
    }
}