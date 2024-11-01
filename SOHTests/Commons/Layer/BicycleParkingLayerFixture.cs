using System;
using System.Data;
using Mars.Common.IO.Csv;
using Mars.Core.Data;
using Mars.Interfaces;
using Mars.Interfaces.Data;
using Mars.Interfaces.Environments;
using Moq;
using SOHModel.Bicycle.Parking;
using SOHModel.Domain.Graph;

namespace SOHTests.Commons.Layer;

/// <summary>
///     Holds the bicycle parking layer for Altona Altstadt
/// </summary>
public class BicycleParkingLayerFixture : IDisposable
{
    public BicycleParkingLayerFixture(ISpatialGraphEnvironment environment)
    {
        DataTable? dataTable = CsvReader.MapData(ResourcesConstants.BicycleCsv);
        EntityManagerImpl manager = new EntityManagerImpl(dataTable);

        Mock<ISimulationContainer> mock = new Mock<ISimulationContainer>();
        mock.Setup(container => container.Resolve<IEntityManager>()).Returns(manager);
        LayerInitData layerInitData = new LayerInitData
        {
            LayerInitConfig = { File = ResourcesConstants.ParkingAltonaAltstadt },
            Container = mock.Object
        };

        SpatialGraphMediatorLayer mediatorLayer = new SpatialGraphMediatorLayer { Environment = environment };
        BicycleParkingLayer = new BicycleParkingLayer { GraphLayer = mediatorLayer };
        BicycleParkingLayer.InitLayer(layerInitData);
    }

    public BicycleParkingLayer BicycleParkingLayer { get; }

    public void Dispose()
    {
        BicycleParkingLayer.Dispose();
    }
}