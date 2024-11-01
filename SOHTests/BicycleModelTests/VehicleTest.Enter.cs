using System;
using SOHModel.Bicycle.Steering;
using Xunit;

namespace SOHTests.BicycleModelTests;

public partial class VehicleTest
{
    [Fact]
    public void EnterDriverAsPassenger()
    {
        TestBicycle vehicle = new TestBicycle(new Random().Next());

        TestBicycleDriver driver = new TestBicycleDriver();
        bool result = vehicle.TryEnterDriver(driver, out BicycleSteeringHandle driverVehicle);
        Assert.True(result);
        Assert.NotNull(driverVehicle);
        Assert.Equal(driver, vehicle.Driver);

        bool result2 = vehicle.TryEnterPassenger(driver, out BicyclePassengerHandle driverVehicle2);
        Assert.False(result2);
        Assert.Null(driverVehicle2);
        Assert.DoesNotContain(driver, vehicle.Passengers);
    }

    [Fact]
    public void EnterDriverInEmptyVehicle()
    {
        TestBicycle vehicle = new TestBicycle(new Random().Next());

        TestBicycleDriver driver = new TestBicycleDriver();
        bool result = vehicle.TryEnterDriver(driver, out BicycleSteeringHandle driverVehicle);
        Assert.True(result);
        Assert.NotNull(driverVehicle);
        Assert.Equal(driver, vehicle.Driver);
    }

    [Fact]
    public void EnterDriverInOccupiedVehicle()
    {
        TestBicycle vehicle = new TestBicycle(new Random().Next());

        TestBicycleDriver firstDriver = new TestBicycleDriver();
        bool result = vehicle.TryEnterDriver(firstDriver, out BicycleSteeringHandle driverVehicle);
        Assert.True(result);
        Assert.NotNull(driverVehicle);
        Assert.Equal(firstDriver, vehicle.Driver);

        TestBicycleDriver secondDriver = new TestBicycleDriver();
        bool result2 = vehicle.TryEnterDriver(secondDriver, out BicycleSteeringHandle driverVehicle2);
        Assert.False(result2);
        Assert.Null(driverVehicle2);
        Assert.NotEqual(secondDriver, vehicle.Driver);
    }

    [Fact]
    public void EnterDriverTwice()
    {
        TestBicycle vehicle = new TestBicycle(new Random().Next());

        TestBicycleDriver driver = new TestBicycleDriver();
        bool result = vehicle.TryEnterDriver(driver, out BicycleSteeringHandle driverVehicle);
        Assert.True(result);
        Assert.NotNull(driverVehicle);
        Assert.Equal(driver, vehicle.Driver);

        bool result2 = vehicle.TryEnterDriver(driver, out BicycleSteeringHandle driverVehicle2);
        Assert.False(result2);
        Assert.Null(driverVehicle2);
        Assert.Equal(driver, vehicle.Driver);
    }

    [Fact]
    public void EnterPassengerAsDriver()
    {
        TestBicycle vehicle = new TestBicycle(1);

        TestBicycleDriver passenger = new TestBicycleDriver();
        bool result = vehicle.TryEnterPassenger(passenger, out BicyclePassengerHandle passengerVehicle);
        Assert.True(result);
        Assert.NotNull(passengerVehicle);
        Assert.Contains(passenger, vehicle.Passengers);

        bool result2 = vehicle.TryEnterDriver(passenger, out BicycleSteeringHandle driverVehicle);
        Assert.False(result2);
        Assert.Null(driverVehicle);
        Assert.NotEqual(passenger, vehicle.Driver);
    }

    [Fact]
    public void EnterPassengerInVehicleWithFreeSeats()
    {
        TestBicycle vehicle = new TestBicycle(2);

        TestBicycleDriver passenger = new TestBicycleDriver();
        bool result = vehicle.TryEnterPassenger(passenger, out BicyclePassengerHandle passengerVehicle);
        Assert.True(result);
        Assert.NotNull(passengerVehicle);
        Assert.Contains(passenger, vehicle.Passengers);
    }

    // TODO should capacity for bicycles should be a parameter?
//        [Fact]
//        public void EnterPassengerInVehicleWithoutCapacity()
//        {
//            var vehicle = new TestVehicle(0);
//
//            var passenger = new TestVehicleDriver();
//            var result = vehicle.TryEnterPassenger(passenger, out var passengerVehicle);
//            Assert.False(result);
//            Assert.Null(passengerVehicle);
//            Assert.DoesNotContain(passenger, vehicle.Passengers);
//        }

    [Fact]
    public void EnterPassengerInVehicleWithoutFreeSeats()
    {
        TestBicycle vehicle = new TestBicycle(1);

        TestBicycleDriver passenger = new TestBicycleDriver();
        bool result = vehicle.TryEnterPassenger(passenger, out BicyclePassengerHandle passengerVehicle);
        Assert.True(result);
        Assert.NotNull(passengerVehicle);
        Assert.Contains(passenger, vehicle.Passengers);

        TestBicycleDriver passenger2 = new TestBicycleDriver();
        bool result2 = vehicle.TryEnterPassenger(passenger2, out BicyclePassengerHandle passengerVehicle2);
        Assert.False(result2);
        Assert.Null(passengerVehicle2);
        Assert.DoesNotContain(passenger2, vehicle.Passengers);
    }

    [Fact]
    public void EnterPassengerTwice()
    {
        TestBicycle vehicle = new TestBicycle(1);

        TestBicycleDriver passenger = new TestBicycleDriver();
        bool result = vehicle.TryEnterPassenger(passenger, out BicyclePassengerHandle passengerVehicle);
        Assert.True(result);
        Assert.NotNull(passengerVehicle);
        Assert.Contains(passenger, vehicle.Passengers);

        bool result2 = vehicle.TryEnterPassenger(passenger, out BicyclePassengerHandle passengerVehicle2);
        Assert.False(result2);
        Assert.Null(passengerVehicle2);
        Assert.Contains(passenger, vehicle.Passengers);
    }
}