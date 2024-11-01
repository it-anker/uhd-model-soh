using System.Data;
using Mars.Common.IO.Csv;
using Mars.Core.Data;
using Mars.Interfaces;
using Mars.Interfaces.Data;
using Mars.Interfaces.Environments;
using Moq;
using SOHModel.Car.Model;
using SOHTests.Commons.Environment;
using Xunit;

namespace SOHTests.CarModelTests;

public class CarLayerTests
{
    [Fact]
    public void InitDataEnvironmentOverwritesConstructorEnvironment()
    {
        ISpatialGraphEnvironment environment = new FourNodeGraphEnv().GraphEnvironment;
        
        CarLayer carLayer = new CarLayer(environment);
        Assert.Equal(environment, carLayer.Environment);

        DataTable? dataTable = CsvReader.MapData(ResourcesConstants.CarCsv);
        EntityManagerImpl manager = new EntityManagerImpl(dataTable);

        Mock<ISimulationContainer> mock = new Mock<ISimulationContainer>();
        mock.Setup(container => container.Resolve<IEntityManager>()).Returns(manager);
        LayerInitData layerInitData = new LayerInitData(SimulationContext.Start2020InSeconds)
        {
            LayerInitConfig =
            {
                File = ResourcesConstants.DriveGraphAltonaAltstadt
            },
            Container = mock.Object
        };
        carLayer.InitLayer(layerInitData, (_, _) => { }, (_, _) => { });
        Assert.NotEqual(environment, carLayer.Environment);
    }

    [Fact]
    public void InitEnvironmentWithConstructor()
    {
        CarLayer carLayer = new CarLayer(new FourNodeGraphEnv().GraphEnvironment);
        Assert.NotNull(carLayer.Environment);
    }

    [Fact]
    public void InitEnvironmentWithInitData()
    {
        CarLayer carLayer = new CarLayer();
        Assert.Null(carLayer.Environment);

        DataTable? dataTable = CsvReader.MapData(ResourcesConstants.CarCsv);
        EntityManagerImpl manager = new EntityManagerImpl(dataTable);

        Mock<ISimulationContainer> mock = new Mock<ISimulationContainer>();
        mock.Setup(container => container.Resolve<IEntityManager>()).Returns(manager);
        LayerInitData initData = new LayerInitData
        {
            LayerInitConfig =
            {
                File = ResourcesConstants.DriveGraphAltonaAltstadt
            },
            Container = mock.Object
        };
        carLayer.InitLayer(initData, (_, _) => { }, (_, _) => { });

        Assert.NotNull(carLayer.Environment);
    }
}