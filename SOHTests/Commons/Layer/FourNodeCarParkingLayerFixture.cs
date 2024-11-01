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
using SOHTests.Commons.Environment;

namespace SOHTests.Commons.Layer;

public class FourNodeCarParkingLayerFixture : IDisposable
{
    public FourNodeCarParkingLayerFixture(ISpatialGraphLayer streetLayer)
    {
        List<IFeature> features = new List<IFeature>
        {
            new VectorStructuredData
            {
                Data = new Dictionary<string, object> { { "area", 0 } },
                Geometry = new Point(FourNodeGraphEnv.Node2Pos.X, FourNodeGraphEnv.Node2Pos.Y)
            },
            new VectorStructuredData
            {
                Data = new Dictionary<string, object> { { "area", 0 } },
                Geometry = new Point(FourNodeGraphEnv.Node2Pos.X, FourNodeGraphEnv.Node2Pos.Y)
            },
            new VectorStructuredData
            {
                Data = new Dictionary<string, object> { { "area", 100 } },
                Geometry = new Point(FourNodeGraphEnv.Node3Pos.X, FourNodeGraphEnv.Node3Pos.Y)
            }
        };
        DataTable? dataTable = new CsvReader(ResourcesConstants.CarCsv, true).ToTable();
        EntityManagerImpl entityManagerImpl = new EntityManagerImpl((typeof(Car), dataTable));
        Mock<ISimulationContainer> mock = new Mock<ISimulationContainer>();

        mock.Setup(container => container.Resolve<IEntityManager>()).Returns(entityManagerImpl);
        LayerInitData mapping = new LayerInitData
        {
            LayerInitConfig = new LayerMapping
            {
                Value = features
            },
            Container = mock.Object
        };

        CarParkingLayer = new CarParkingLayer { StreetLayer = streetLayer };
        CarParkingLayer.InitLayer(mapping);
    }

    public CarParkingLayer CarParkingLayer { get; }

    public void Dispose()
    {
        CarParkingLayer.Dispose();
    }
}