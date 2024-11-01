using System;
using System.Data;
using Mars.Common.IO.Csv;
using Mars.Core.Data;
using Mars.Interfaces;
using Mars.Interfaces.Data;
using Moq;
using SOHModel.Car.Parking;
using SOHModel.Domain.Graph;

namespace SOHTests.Commons.Layer;

/// <summary>
///     Holds the car parking layer for Altona Altstadt
/// </summary>
public class CarParkingLayerFixture : IDisposable
{
    public CarParkingLayerFixture(ISpatialGraphLayer streetLayer)
    {
        DataTable? dataTable = CsvReader.MapData(ResourcesConstants.CarCsv);
        EntityManagerImpl manager = new EntityManagerImpl(dataTable);

        Mock<ISimulationContainer> mock = new Mock<ISimulationContainer>();
        mock.Setup(container => container.Resolve<IEntityManager>()).Returns(manager);
        LayerInitData layerInitData = new LayerInitData
        {
            LayerInitConfig = { File = ResourcesConstants.ParkingAltonaAltstadt },
            Container = mock.Object
        };

        CarParkingLayer = new CarParkingLayer { StreetLayer = streetLayer };
        CarParkingLayer.InitLayer(layerInitData);
    }

    public CarParkingLayer CarParkingLayer { get; }

    public void Dispose()
    {
        CarParkingLayer.Dispose();
    }
}