using System.Collections.Generic;
using System.Linq;
using Mars.Common.IO.Csv;
using Mars.Core.Data;
using Mars.Interfaces;
using Mars.Interfaces.Data;
using Mars.Interfaces.Model;
using Moq;
using SOHModel.Ferry.Model;
using SOHTests.Commons.Layer;
using Xunit;

namespace SOHTests.FerryModelTests;

public class FerryModelStartingTests : IClassFixture<FerryRouteLayerFixture>
{
    private readonly FerryRouteLayerFixture _routeLayerFixture;

    public FerryModelStartingTests(FerryRouteLayerFixture routeLayerFixture)
    {
        _routeLayerFixture = routeLayerFixture;
    }

    [Fact]
    public void TestStartupFerryEngine()
    {
        FerryLayer layer = new FerryLayer(_routeLayerFixture.FerryRouteLayer);
        bool isRegistered = false;

        SimulationContext? context = SimulationContext.Start2020InSeconds;


        EntityManagerImpl manager = new EntityManagerImpl(CsvReader.MapData(ResourcesConstants.FerryCsv));

        Mock<ISimulationContainer> containerMock = new Mock<ISimulationContainer>();
        containerMock.Setup(container => container.Resolve<IEntityManager>()).Returns(() => manager);

        Assert.True(layer.InitLayer(new LayerInitData
            {
                LayerInitConfig = new LayerMapping
                {
                    File = ResourcesConstants.FerryGraph
                },
                Container = containerMock.Object, AgentInitConfigs = new[]
                {
                    new AgentMapping
                    {
                        File = ResourcesConstants.FerryDriverCsv, InstanceCount = 1,
                        DataContainer = new DataContainer
                        {
                            DataTable = CsvReader.MapData(ResourcesConstants.FerryDriverCsv)
                        },
                        IndividualMapping = new List<IndividualMapping>
                        {
                            new()
                            {
                                Name = "Line", FieldName = "line"
                            }
                        },
                        Type = new AgentType(typeof(FerryDriver))
                        {
                            LayerReference = new LayerType(typeof(FerryLayer))
                        }
                    }
                }
            }, (_, _) => isRegistered = true,
            (_, _) => { }));

        layer.Context = context;

        Assert.True(isRegistered);
        Assert.Single(layer.Driver);
        FerryDriver driver = layer.Driver.Values.First();
        Assert.Equal(61, driver.Line);

        for (int i = 0; i < 5000; i++, context.UpdateStep())
            foreach (KeyValuePair<Guid, FerryDriver> ferryDriver in layer.Driver)
                ferryDriver.Value.Tick();
    }
}