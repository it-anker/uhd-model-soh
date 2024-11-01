using Mars.Components.Environments;
using Mars.Interfaces.Data;
using Mars.Interfaces.Environments;
using SOHModel.Bicycle.Rental;
using SOHModel.Domain.Graph;
using Xunit;

namespace SOHTests.BicycleModelTests;

public class BicycleRentalStationTests
{
    private readonly BicycleRentalLayer _bicycleRentalLayer;

    public BicycleRentalStationTests()
    {
        SpatialGraphEnvironment environment = new SpatialGraphEnvironment(ResourcesConstants.DriveGraphAltonaAltstadt);

        _bicycleRentalLayer = new BicycleRentalLayer
        {
            KeyCount = "anzahl_raeder",
            SpatialGraphMediatorLayer = new SpatialGraphMediatorLayer { Environment = environment }
        };

        _bicycleRentalLayer.InitLayer(new LayerInitData
        {
            LayerInitConfig = { File = ResourcesConstants.BicycleRentalAltonaAltstadt }
        });
    }

    [Fact]
    public void InitRentalPointsFromVectorFile()
    {
        Position? pos = Position.CreateGeoPosition(9.949921, 53.554514);
        BicycleRentalStation? bicycleRentalPoint = _bicycleRentalLayer.Nearest(pos, false);
        Assert.NotNull(bicycleRentalPoint);
        Assert.Equal("Holstenstraße / Thadenstraße", bicycleRentalPoint!.Name);
        Assert.False(bicycleRentalPoint.Empty);
        Assert.Equal(2, bicycleRentalPoint.Count);
    }

    [Fact]
    public void RentBicyclesUntilRentalPointIsEmpty()
    {
        Position? pos = Position.CreateGeoPosition(9.946307, 53.543901);
        BicycleRentalStation? bicycleRentalPoint = _bicycleRentalLayer.Nearest(pos, false);

        Assert.NotNull(bicycleRentalPoint);
        Assert.Equal("Van-der-Smissen-Straße / Große Elbstraße", bicycleRentalPoint!.Name);

        const int startCount = 15;
        Assert.Equal(startCount, bicycleRentalPoint.Count);

        for (int i = 0; i < startCount; i++)
        {
            IRentalBicycle? rentalBicycle = bicycleRentalPoint.RentAny();
            Assert.NotNull(rentalBicycle);
            Assert.True(bicycleRentalPoint.Leave(rentalBicycle!));
        }

        Assert.Equal(0, bicycleRentalPoint.Count);
        Assert.True(bicycleRentalPoint.Empty);

        // no further rental possible
        Assert.Null(bicycleRentalPoint.RentAny());
    }
}