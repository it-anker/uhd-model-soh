using System.Data;
using Mars.Common.IO.Csv;
using Mars.Core.Data;
using Mars.Interfaces;
using Mars.Interfaces.Data;
using Mars.Interfaces.Environments;
using Moq;
using SOHModel.Car.Model;

namespace SOHTests.Commons.Layer;

public class CarLayerFixture
{
    public CarLayerFixture(ISpatialGraphEnvironment environment = null)
    {
        SimulationContext? simulationContext = SimulationContext.Start2020InSeconds;
        CarLayer = new CarLayer(environment);

        DataTable? dataTable = CsvReader.MapData(ResourcesConstants.CarCsv);
        EntityManagerImpl manager = new EntityManagerImpl(dataTable);

        Mock<ISimulationContainer> mock = new Mock<ISimulationContainer>();
        mock.Setup(container => container.Resolve<IEntityManager>()).Returns(manager);
        LayerInitData layerInitData = new LayerInitData(simulationContext)
        {
            Container = mock.Object
        };
        CarLayer.InitLayer(layerInitData, (_, _) => { }, (_, _) => { });
        CarLayer.CarParkingLayer = new CarParkingLayerFixture(CarLayer).CarParkingLayer;
    }

    public CarLayer CarLayer { get; }
}