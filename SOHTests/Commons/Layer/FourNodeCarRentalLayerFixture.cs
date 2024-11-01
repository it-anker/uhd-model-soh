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
using SOHModel.Car.Rental;
using SOHModel.Domain.Graph;
using SOHTests.Commons.Environment;

namespace SOHTests.Commons.Layer;

public class FourNodeCarRentalLayerFixture
{
    public FourNodeCarRentalLayerFixture()
    {
        FourNodeGraphEnv = new FourNodeGraphEnv();

        List<IFeature> features = new List<IFeature>
        {
            new VectorStructuredData
            {
                Data = new Dictionary<string, object>(),
                Geometry = new Point(FourNodeGraphEnv.Node2Pos.X, FourNodeGraphEnv.Node2Pos.Y)
            },
            new VectorStructuredData
            {
                Data = new Dictionary<string, object>(),
                Geometry = new Point(FourNodeGraphEnv.Node3Pos.X, FourNodeGraphEnv.Node3Pos.Y)
            }
        };
        DataTable? dataTable = new CsvReader(ResourcesConstants.CarCsv, true).ToTable();
        EntityManagerImpl entityManagerImpl = new EntityManagerImpl((typeof(RentalCar), dataTable));
        Mock<ISimulationContainer> mock = new Mock<ISimulationContainer>();

        mock.Setup(container => container.Resolve<IEntityManager>()).Returns(entityManagerImpl);
        LayerInitData mapping = new LayerInitData
        {
            LayerInitConfig = new LayerMapping
            {
                Value = features,
                IndividualMapping = new List<IndividualMapping>
                {
                    new() { Name = "carKeyAttributeName", Value = "type" },
                    new() { Name = "carValueToMatch", Value = "Golf" }
                }
            },
            Container = mock.Object
        };

        StreetLayer streetLayer = new StreetLayer { Environment = FourNodeGraphEnv.GraphEnvironment };
        CarRentalLayer carRentalLayer = new CarRentalLayer { StreetLayer = streetLayer };
        carRentalLayer.InitLayer(mapping, (_, _) => { }, (_, _) => { });

        CarRentalLayer = carRentalLayer;
    }

    public FourNodeGraphEnv FourNodeGraphEnv { get; }
    public ICarRentalLayer CarRentalLayer { get; }
}