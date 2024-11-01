using System;
using SOHModel.Bicycle.Steering;
using SOHModel.Car.Steering;
using Xunit;

namespace SOHTests.DomainTests.VehicleTests;

public partial class VehicleTest
{
    [Fact]
    public void LeaveBicycleAsDriver()
    {
        TestBicycleWithoutRangeCheck vehicle = new TestBicycleWithoutRangeCheck();

        TestSteeringCapable driver = new TestSteeringCapable();
        bool result = vehicle.TryEnterDriver(driver, out BicycleSteeringHandle driverVehicle);
        Assert.True(result);
        Assert.NotNull(driverVehicle);
        Assert.Equal(driver, vehicle.Driver);

        bool result2 = driverVehicle.LeaveVehicle(driver);
        Assert.True(result2);
    }

    [Fact]
    public void LeaveCarAsDriver()
    {
        TestCarWithoutRangeCheck vehicle = new TestCarWithoutRangeCheck(_environment, new Random().Next());

        TestSteeringCapable driver = new TestSteeringCapable();
        bool result = vehicle.TryEnterDriver(driver, out CarSteeringHandle driverVehicle);
        Assert.True(result);
        Assert.NotNull(driverVehicle);
        Assert.Equal(driver, vehicle.Driver);

        bool result2 = driverVehicle.LeaveVehicle(driver);
        Assert.True(result2);
    }

    [Fact]
    public void LeaveVehicleAsDriverPassengersAreInformed()
    {
        TestCarWithoutRangeCheck vehicle = new TestCarWithoutRangeCheck(_environment, 1);

        TestSteeringCapable driver = new TestSteeringCapable();
        bool result = vehicle.TryEnterDriver(driver, out CarSteeringHandle driverVehicle);
        Assert.True(result);
        Assert.NotNull(driverVehicle);
        Assert.Equal(driver, vehicle.Driver);

        TestSteeringCapable passenger = new TestSteeringCapable();
        passenger.CarWithoutRangeCheck = vehicle;
        bool result2 = vehicle.TryEnterPassenger(passenger, out CarPassengerHandle passengerVehicle);
        Assert.True(result2);
        Assert.NotNull(passengerVehicle);
        Assert.Contains(passenger, vehicle.Passengers);

        bool result3 = driverVehicle.LeaveVehicle(driver);
        Assert.True(result3);
    }

    [Fact]
    public void LeaveVehicleAsDriverWhichIsNotTheDriver()
    {
        TestCarWithoutRangeCheck vehicle = new TestCarWithoutRangeCheck(_environment, new Random().Next());

        TestSteeringCapable driver = new TestSteeringCapable();
        bool result = vehicle.TryEnterDriver(driver, out CarSteeringHandle driverVehicle);
        Assert.True(result);
        Assert.NotNull(driverVehicle);
        Assert.Equal(driver, vehicle.Driver);

        bool result2 = driverVehicle.LeaveVehicle(new TestSteeringCapable());
        Assert.False(result2);
    }

    [Fact]
    public void LeaveVehicleAsPassenger()
    {
        TestCarWithoutRangeCheck vehicle = new TestCarWithoutRangeCheck(_environment, 1);

        TestSteeringCapable passenger = new TestSteeringCapable();
        bool result = vehicle.TryEnterPassenger(passenger, out CarPassengerHandle passengerVehicle);
        Assert.True(result);
        Assert.NotNull(passengerVehicle);
        Assert.Contains(passenger, vehicle.Passengers);

        bool result2 = passengerVehicle.LeaveVehicle(passenger);
        Assert.True(result2);
    }


    [Fact]
    public void LeaveVehicleAsPassengerThatIsNoPassenger()
    {
        TestCarWithoutRangeCheck vehicle = new TestCarWithoutRangeCheck(_environment, 10);

        TestSteeringCapable passenger = new TestSteeringCapable();
        bool result = vehicle.TryEnterPassenger(passenger, out CarPassengerHandle passengerVehicle);
        Assert.True(result);
        Assert.NotNull(passengerVehicle);
        Assert.Contains(passenger, vehicle.Passengers);

        bool result2 = passengerVehicle.LeaveVehicle(new TestSteeringCapable());
        Assert.False(result2);
    }
}