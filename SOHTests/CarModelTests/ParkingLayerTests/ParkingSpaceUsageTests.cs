using System;
using System.Collections.Generic;
using System.Data;
using Mars.Common.IO.Csv;
using Mars.Core.Data;
using Mars.Interfaces;
using Mars.Interfaces.Data;
using Mars.Interfaces.Model;
using Moq;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using SOHModel.Car.Model;
using SOHModel.Car.Parking;
using SOHModel.Domain.Graph;
using Xunit;
using Position = Mars.Interfaces.Environments.Position;

namespace SOHTests.CarModelTests.ParkingLayerTests;

public class ParkingSpaceUsageTests
{
    private static readonly LayerInitData Mapping;
    private readonly CarParkingLayer _carParkingLayer = new() { StreetLayer = new StreetLayer() };

    static ParkingSpaceUsageTests()
    {
        List<IFeature> features = new List<IFeature>
        {
            new VectorStructuredData
            {
                Data = new Dictionary<string, object> { { "area", 0 } },
                Geometry = new Point(10.00553, 53.56310)
            },
            new VectorStructuredData
            {
                Data = new Dictionary<string, object> { { "area", 0 } },
                Geometry = new Point(10.00738, 53.56693)
            },
            new VectorStructuredData
            {
                Data = new Dictionary<string, object> { { "area", 0 } },
                Geometry = new Point(10.00881, 53.56377)
            },
            new VectorStructuredData
            {
                Data = new Dictionary<string, object> { { "area", 0 } },
                Geometry = new Point(9.8968278, 53.55155)
            }
        };
        DataTable? dataTable = CsvReader.MapData(ResourcesConstants.CarCsv);
        EntityManagerImpl manager = new EntityManagerImpl(dataTable);

        Mock<ISimulationContainer> mock = new Mock<ISimulationContainer>();
        mock.Setup(container => container.Resolve<IEntityManager>()).Returns(manager);
        Mapping = new LayerInitData
        {
            LayerInitConfig = new LayerMapping
            {
                Value = features
            },
            Container = mock.Object
        };
    }

    public ParkingSpaceUsageTests()
    {
        _carParkingLayer.InitLayer(Mapping);
    }

    [Fact]
    public void FindNextFreeParkingSpot()
    {
        CarParkingSpace firstParkingSpace = new CarParkingSpace();
        Point point = new Point(9.8885117, 53.5597505);
        firstParkingSpace.Init(null, new VectorStructuredData
        {
            Geometry = point,
            Data = new Dictionary<string, object> { { "area", 0 } }
        });

        Assert.True(firstParkingSpace.HasCapacity);
        Assert.Equal(1, firstParkingSpace.Capacity);

        firstParkingSpace.Init(null, new VectorStructuredData
        {
            Geometry = point,
            Data = new Dictionary<string, object> { { "area", 399 } }
        });

        Assert.Equal(26, firstParkingSpace.Capacity);

        firstParkingSpace.Init(null, new VectorStructuredData
        {
            Geometry = point,
            Data = new Dictionary<string, object> { { "area", 600 } }
        });

        Assert.Equal(30, firstParkingSpace.Capacity);

        firstParkingSpace.Init(null, new VectorStructuredData
        {
            Geometry = point,
            Data = new Dictionary<string, object> { { "area", -100 } }
        });
        Assert.Equal(1, firstParkingSpace.Capacity);

        Assert.False(firstParkingSpace.Occupied);
        Assert.NotNull(firstParkingSpace.ParkingVehicles);
        Assert.Empty(firstParkingSpace.ParkingVehicles);
        Assert.NotNull(firstParkingSpace.VectorStructured);

        Golf golf = Golf.Create(_carParkingLayer);
        Assert.True(firstParkingSpace.Enter(golf));

        Assert.Single(firstParkingSpace.ParkingVehicles);
        Assert.False(firstParkingSpace.Occupied);
        Assert.Equal(9.8885117, firstParkingSpace.Position.X, 6);
        Assert.Equal(53.5597505, firstParkingSpace.Position.Y, 6);

        Assert.False(firstParkingSpace.HasCapacity);
    }

    [Fact]
    public void TestEnterLeaveParkingSpace()
    {
        Point point = new Point(9.8885117, 53.5597505);
        CarParkingSpace firstParkingSpace = new CarParkingSpace();

        firstParkingSpace.Init(null, new VectorStructuredData
        {
            Geometry = point,
            Data = new Dictionary<string, object> { { "area", 30 } }
        });

        Assert.Equal(2, firstParkingSpace.Capacity);

        Golf golf = Golf.Create(_carParkingLayer);
        Golf golf2 = Golf.Create(_carParkingLayer);
        Golf golf3 = Golf.Create(_carParkingLayer);
        Assert.True(firstParkingSpace.Enter(golf));

        Assert.Equal(firstParkingSpace, golf.CarParkingSpace);
        Assert.Single(firstParkingSpace.ParkingVehicles);
        Assert.False(firstParkingSpace.Occupied);
        Assert.True(firstParkingSpace.HasCapacity);

        Assert.True(firstParkingSpace.Enter(golf2));

        Assert.Equal(2, firstParkingSpace.ParkingVehicles.Count);
        Assert.False(firstParkingSpace.HasCapacity);

        Assert.False(firstParkingSpace.Enter(golf3));

        Assert.True(firstParkingSpace.Leave(golf));
        Assert.Null(golf.CarParkingSpace);
        Assert.True(firstParkingSpace.HasCapacity);
        Assert.True(firstParkingSpace.Enter(golf3));

        Assert.True(firstParkingSpace.Leave(golf3));

        firstParkingSpace.Occupied = true;
        Assert.False(firstParkingSpace.Enter(golf3));
    }

    [Fact]
    public void TestException()
    {
        Point point = new Point(9.8885117, 53.5597505);
        CarParkingSpace firstParkingSpace = new CarParkingSpace();
        CarParkingSpace secondParkingSpace = new CarParkingSpace();

        firstParkingSpace.Init(null, new VectorStructuredData
        {
            Geometry = point,
            Data = new Dictionary<string, object> { { "area", 0 } }
        });

        secondParkingSpace.Init(null, new VectorStructuredData
        {
            Geometry = point,
            Data = new Dictionary<string, object> { { "area", 0 } }
        });

        Golf golf = Golf.Create(_carParkingLayer);
        Assert.True(firstParkingSpace.Enter(golf));
        Assert.Throws<ArgumentException>(() => firstParkingSpace.Enter(golf));
        Assert.Throws<ArgumentException>(() => secondParkingSpace.Enter(golf));
        Assert.True(firstParkingSpace.Leave(golf));
        Assert.Throws<ArgumentException>(() => firstParkingSpace.Leave(golf));
    }

    [Fact]
    public void TestFindFreeEnterAndLeaveParkingSpace()
    {
        Position? position = Position.CreatePosition(9.931294, 53.554248);

        CarParkingSpace? parkingSpace = _carParkingLayer.Nearest(position);
        Assert.NotNull(parkingSpace);
        Assert.True(parkingSpace!.HasCapacity);

        Assert.Equal(1, parkingSpace.Capacity);
        Golf golf = Golf.Create(_carParkingLayer);
        Assert.True(parkingSpace.Enter(golf));

        CarParkingSpace? parkingSpace2 = _carParkingLayer.Nearest(position);
        Assert.NotEqual(parkingSpace, parkingSpace2);
        Assert.NotNull(parkingSpace2);
        Golf golf2 = Golf.Create(_carParkingLayer);
        Assert.True(parkingSpace2!.Enter(golf2));

        CarParkingSpace? parkingSpace3 = _carParkingLayer.Nearest(position);
        Assert.NotEqual(parkingSpace, parkingSpace3);
        Assert.NotEqual(parkingSpace2, parkingSpace3);
        Assert.NotNull(parkingSpace3);
        Golf golf3 = Golf.Create(_carParkingLayer);
        Assert.True(parkingSpace3!.Enter(golf3));

        CarParkingSpace? parkingSpace4 = _carParkingLayer.Nearest(position);
        Assert.NotEqual(parkingSpace4, parkingSpace);
        Assert.NotEqual(parkingSpace4, parkingSpace2);
        Assert.NotEqual(parkingSpace4, parkingSpace3);
        Assert.NotNull(parkingSpace4);
        Golf golf4 = Golf.Create(_carParkingLayer);
        Assert.True(parkingSpace4!.Enter(golf4));

        Assert.Null(_carParkingLayer.Nearest(position));
    }
}