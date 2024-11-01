using System.Data;
using Mars.Common.IO.Csv;
using Mars.Core.Data;
using SOHModel.Car.Model;
using Xunit;

namespace SOHTests.CarModelTests;

public class CarEntityTests
{
    [Fact]
    public void ReadEntityCsv()
    {
        DataTable? data = CsvReader.MapData(ResourcesConstants.CarCsv);
        DataRow dataRow = data.Rows[0];
        int index = data.Columns.IndexOf("maxSpeed");

        Assert.Equal("13.89", dataRow.ItemArray[index]);
    }

    [Fact]
    public void ReadEntityCsvByEntityManager()
    {
        DataTable? data = CsvReader.MapData(ResourcesConstants.CarCsv);

        EntityManagerImpl entityManagerImpl = new EntityManagerImpl(data);
        Car? car = entityManagerImpl.Create<Car>("type", "Golf");

        Assert.Equal(13.89, car.MaxSpeed);
    }
}