using System;
using System.Data;
using Mars.Common.IO.Csv;
using Mars.Core.Data;
using SOHModel.Bicycle.Common;
using SOHModel.Bicycle.Model;
using Xunit;

namespace SOHTests.BicycleModelTests;

public class CyclistTest
{
    [Fact]
    public void TestCreateBicycleByEntityManager()
    {
        DataTable? dataTable = CsvReader.MapData(ResourcesConstants.BicycleCsv);
        EntityManagerImpl manager = new EntityManagerImpl(dataTable);

        Bicycle? bicycle = manager.Create<Bicycle>("type", "city");

        Assert.NotNull(bicycle);
        Assert.Equal(BicycleType.City, bicycle.Type);
        Assert.Equal(0.60, bicycle.Width);
        Assert.NotEqual(0, bicycle.Weight);
        Assert.Equal(75, bicycle.DriverMass);
        Assert.Equal(3.0, bicycle.MaxAcceleration);
        Assert.Equal(3.0, bicycle.MaxDeceleration);
    }

    [Fact]
    public void TestEntityManagerException()
    {
        DataTable? dataTable = CsvReader.MapData(ResourcesConstants.BicycleCsv);
        EntityManagerImpl manager = new EntityManagerImpl(dataTable);

        Assert.Throws<ArgumentException>(() =>
            manager.Create<Bicycle>("bicycleType", "city"));
    }
}