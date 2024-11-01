using System;
using System.Data;
using Mars.Common.IO.Csv;
using Mars.Core.Data;
using Mars.Interfaces.Data;
using Mars.Interfaces.Environments;
using SOHModel.Bicycle.Model;
using SOHModel.Car.Model;
using Xunit;

namespace SOHTests.DomainTests.RoutingTests;

public class RoutingTests
{
    private readonly CarLayer _carLayer;
    private readonly ISpatialGraphEnvironment _environment;

    public RoutingTests()
    {
        DataTable? dataTableBicycle = CsvReader.MapData(ResourcesConstants.BicycleCsv);
        DataTable? dataTableCar = CsvReader.MapData(ResourcesConstants.CarCsv);
        EntityManagerImpl manager =
            new EntityManagerImpl((typeof(Bicycle), dataTableBicycle), (typeof(Car), dataTableCar));

        _carLayer = new CarLayer();
        LayerInitData initData = new LayerInitData
            { LayerInitConfig = { File = ResourcesConstants.DriveGraphAltonaAltstadt } };
        _carLayer.InitLayer(initData, (_, _) => { }, (_, _) => { });

        _carLayer.EntityManager = manager;
        _environment = _carLayer.Environment;
    }

    [Fact]
    public void TravelTimeHeuristicForSlowVsFastCar()
    {
        ISpatialNode? start = _environment.NearestNode(Position.CreateGeoPosition(9.954986699999999, 53.56093));
        ISpatialNode? goal = _environment.NearestNode(Position.CreateGeoPosition(9.9360853, 53.5503159));
        Assert.InRange(start.Position.DistanceInKmTo(goal.Position), 1, 2);

        Car? slowCar = _carLayer.EntityManager.Create<Car>("type", "Golf");
        slowCar.Environment = _environment;
        slowCar.MaxSpeed = 30 / 3.6;

        Car? fastCar = _carLayer.EntityManager.Create<Car>("type", "Golf");
        fastCar.Environment = _environment;
        fastCar.MaxSpeed = 50 / 3.6;

        Route? routeWithSlowCar = _environment.FindRoute(start, goal, TravelTimeHeuristicFor(slowCar.MaxSpeed));
        Route? routeWithFastCar = _environment.FindRoute(start, goal, TravelTimeHeuristicFor(fastCar.MaxSpeed));

        Assert.NotEqual(routeWithSlowCar, routeWithFastCar);
    }

    /// <summary>
    ///     Travel time heuristic, can be used in conjunction with route planning.
    /// </summary>
    /// <param name="vehicleMaxSpeed">The max speed of the entity in m/s</param>
    /// <returns>Returns a new heuristic to distinguish <see cref="ISpatialEdge" /> according to the time to pass them.</returns>
    public static Func<ISpatialNode, ISpatialEdge, ISpatialNode, double> TravelTimeHeuristicFor(
        double vehicleMaxSpeed)
    {
        return (_, edge, _) => edge.Length / Math.Min(edge.MaxSpeed, vehicleMaxSpeed);
    } // TODO in IRoutePlanner ziehen?
}