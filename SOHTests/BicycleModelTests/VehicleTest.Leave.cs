using System;
using SOHModel.Bicycle.Steering;
using Xunit;

namespace SOHTests.BicycleModelTests;

public partial class VehicleTest
{
    [Fact]
    public void LeaveVehicleAsDriver()
    {
        TestBicycle vehicle = new TestBicycle(new Random().Next());

        TestBicycleDriver driver = new TestBicycleDriver();
        bool result = vehicle.TryEnterDriver(driver, out BicycleSteeringHandle driverVehicle);
        Assert.True(result);
        Assert.NotNull(driverVehicle);
        Assert.Equal(driver, vehicle.Driver);

        bool result2 = driverVehicle.LeaveVehicle(driver);
        Assert.True(result2);
    }

    [Fact]
    public void LeaveVehicleAsDriverPassengersAreInformed()
    {
        TestBicycle vehicle = new TestBicycle(1);

        TestBicycleDriver driver = new TestBicycleDriver();
        bool result = vehicle.TryEnterDriver(driver, out BicycleSteeringHandle driverVehicle);
        Assert.True(result);
        Assert.NotNull(driverVehicle);
        Assert.Equal(driver, vehicle.Driver);

        TestBicycleDriver passenger = new TestBicycleDriver();
        passenger.Bicycle = vehicle;
        bool result2 = vehicle.TryEnterPassenger(passenger, out BicyclePassengerHandle passengerVehicle);
        Assert.True(result2);
        Assert.NotNull(passengerVehicle);
        Assert.Contains(passenger, vehicle.Passengers);

        bool result3 = driverVehicle.LeaveVehicle(driver);
        Assert.True(result3);
    }

    [Fact]
    public void LeaveVehicleAsDriverWhichIsNotTheDriver()
    {
        TestBicycle vehicle = new TestBicycle(new Random().Next());

        TestBicycleDriver driver = new TestBicycleDriver();
        bool result = vehicle.TryEnterDriver(driver, out BicycleSteeringHandle driverVehicle);
        Assert.True(result);
        Assert.NotNull(driverVehicle);
        Assert.Equal(driver, vehicle.Driver);

        bool result2 = driverVehicle.LeaveVehicle(new TestBicycleDriver());
        Assert.False(result2);
    }

    [Fact]
    public void LeaveVehicleAsPassenger()
    {
        TestBicycle vehicle = new TestBicycle(1);

        TestBicycleDriver passenger = new TestBicycleDriver();
        bool result = vehicle.TryEnterPassenger(passenger, out BicyclePassengerHandle passengerVehicle);
        Assert.True(result);
        Assert.NotNull(passengerVehicle);
        Assert.Contains(passenger, vehicle.Passengers);

        bool result2 = passengerVehicle.LeaveVehicle(passenger);
        Assert.True(result2);
    }

    [Fact]
    public void LeaveVehicleAsPassengerThatIsNoPassenger()
    {
        TestBicycle vehicle = new TestBicycle(10);

        TestBicycleDriver passenger = new TestBicycleDriver();
        bool result = vehicle.TryEnterPassenger(passenger, out BicyclePassengerHandle passengerVehicle);
        Assert.True(result);
        Assert.NotNull(passengerVehicle);
        Assert.Contains(passenger, vehicle.Passengers);

        bool result2 = passengerVehicle.LeaveVehicle(new TestBicycleDriver());
        Assert.False(result2);
    }
}