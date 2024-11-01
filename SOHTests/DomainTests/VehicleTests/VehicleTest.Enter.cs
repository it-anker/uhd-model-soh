using System;
using SOHModel.Car.Steering;
using Xunit;

namespace SOHTests.DomainTests.VehicleTests;

public partial class VehicleTest
{
    [Fact]
    public void EnterDriverAsPassenger()
    {
        TestCarWithoutRangeCheck vehicle = new TestCarWithoutRangeCheck(_environment, new Random().Next());

        TestSteeringCapable driver = new TestSteeringCapable();
        bool result = vehicle.TryEnterDriver(driver, out CarSteeringHandle driverVehicle);
        Assert.True(result);
        Assert.NotNull(driverVehicle);
        Assert.Equal(driver, vehicle.Driver);

        bool result2 = vehicle.TryEnterPassenger(driver, out CarPassengerHandle driverVehicle2);
        Assert.False(result2);
        Assert.Null(driverVehicle2);
        Assert.DoesNotContain(driver, vehicle.Passengers);
    }

    [Fact]
    public void EnterDriverInEmptyVehicle()
    {
        TestCarWithoutRangeCheck vehicle = new TestCarWithoutRangeCheck(_environment, new Random().Next());

        TestSteeringCapable driver = new TestSteeringCapable();
        bool result = vehicle.TryEnterDriver(driver, out CarSteeringHandle driverVehicle);
        Assert.True(result);
        Assert.NotNull(driverVehicle);
        Assert.Equal(driver, vehicle.Driver);
    }

    [Fact]
    public void EnterDriverInOccupiedVehicle()
    {
        TestCarWithoutRangeCheck vehicle = new TestCarWithoutRangeCheck(_environment, new Random().Next());

        TestSteeringCapable firstDriver = new TestSteeringCapable();
        bool result = vehicle.TryEnterDriver(firstDriver, out CarSteeringHandle driverVehicle);
        Assert.True(result);
        Assert.NotNull(driverVehicle);
        Assert.Equal(firstDriver, vehicle.Driver);

        TestSteeringCapable secondDriver = new TestSteeringCapable();
        bool result2 = vehicle.TryEnterDriver(secondDriver, out CarSteeringHandle driverVehicle2);
        Assert.False(result2);
        Assert.Null(driverVehicle2);
        Assert.NotEqual(secondDriver, vehicle.Driver);
    }

    [Fact]
    public void EnterDriverTwice()
    {
        TestCarWithoutRangeCheck vehicle = new TestCarWithoutRangeCheck(_environment, new Random().Next());

        TestSteeringCapable driver = new TestSteeringCapable();
        bool result = vehicle.TryEnterDriver(driver, out CarSteeringHandle driverVehicle);
        Assert.True(result);
        Assert.NotNull(driverVehicle);
        Assert.Equal(driver, vehicle.Driver);

        bool result2 = vehicle.TryEnterDriver(driver, out CarSteeringHandle driverVehicle2);
        Assert.False(result2);
        Assert.Null(driverVehicle2);
        Assert.Equal(driver, vehicle.Driver);
    }

    [Fact]
    public void EnterPassengerAsDriver()
    {
        TestCarWithoutRangeCheck vehicle = new TestCarWithoutRangeCheck(_environment, 1);

        TestSteeringCapable passenger = new TestSteeringCapable();
        bool result = vehicle.TryEnterPassenger(passenger, out CarPassengerHandle passengerVehicle);
        Assert.True(result);
        Assert.NotNull(passengerVehicle);
        Assert.Contains(passenger, vehicle.Passengers);

        bool result2 = vehicle.TryEnterDriver(passenger, out CarSteeringHandle driverVehicle);
        Assert.False(result2);
        Assert.Null(driverVehicle);
        Assert.NotEqual(passenger, vehicle.Driver);
    }

    [Fact]
    public void EnterPassengerInVehicleWithFreeSeats()
    {
        TestCarWithoutRangeCheck vehicle = new TestCarWithoutRangeCheck(_environment, 2);

        TestSteeringCapable passenger = new TestSteeringCapable();
        bool result = vehicle.TryEnterPassenger(passenger, out CarPassengerHandle passengerVehicle);
        Assert.True(result);
        Assert.NotNull(passengerVehicle);
        Assert.Contains(passenger, vehicle.Passengers);

        TestSteeringCapable passenger2 = new TestSteeringCapable();
        bool result2 = vehicle.TryEnterPassenger(passenger2, out CarPassengerHandle passengerVehicle2);
        Assert.True(result2);
        Assert.NotNull(passengerVehicle2);
        Assert.Contains(passenger2, vehicle.Passengers);
    }

    [Fact]
    public void EnterPassengerInVehicleWithoutCapacity()
    {
        TestCarWithoutRangeCheck vehicle = new TestCarWithoutRangeCheck(_environment, 0);

        TestSteeringCapable passenger = new TestSteeringCapable();
        bool result = vehicle.TryEnterPassenger(passenger, out CarPassengerHandle passengerVehicle);
        Assert.False(result);
        Assert.Null(passengerVehicle);
        Assert.DoesNotContain(passenger, vehicle.Passengers);
    }

    [Fact]
    public void EnterPassengerInVehicleWithoutFreeSeats()
    {
        TestCarWithoutRangeCheck vehicle = new TestCarWithoutRangeCheck(_environment, 1);

        TestSteeringCapable passenger = new TestSteeringCapable();
        bool result = vehicle.TryEnterPassenger(passenger, out CarPassengerHandle passengerVehicle);
        Assert.True(result);
        Assert.NotNull(passengerVehicle);
        Assert.Contains(passenger, vehicle.Passengers);

        TestSteeringCapable passenger2 = new TestSteeringCapable();
        bool result2 = vehicle.TryEnterPassenger(passenger2, out CarPassengerHandle passengerVehicle2);
        Assert.False(result2);
        Assert.Null(passengerVehicle2);
        Assert.DoesNotContain(passenger2, vehicle.Passengers);
    }

    [Fact]
    public void EnterPassengerTwice()
    {
        TestCarWithoutRangeCheck vehicle = new TestCarWithoutRangeCheck(_environment, 1);

        TestSteeringCapable passenger = new TestSteeringCapable();
        bool result = vehicle.TryEnterPassenger(passenger, out CarPassengerHandle passengerVehicle);
        Assert.True(result);
        Assert.NotNull(passengerVehicle);
        Assert.Contains(passenger, vehicle.Passengers);

        bool result2 = vehicle.TryEnterPassenger(passenger, out CarPassengerHandle passengerVehicle2);
        Assert.False(result2);
        Assert.Null(passengerVehicle2);
        Assert.Contains(passenger, vehicle.Passengers);
    }
}