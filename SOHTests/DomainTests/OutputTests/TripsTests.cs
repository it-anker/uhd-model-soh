using System.Linq;
using Mars.Core.Data.Wrapper.Memory;
using Mars.Interfaces;
using Mars.Interfaces.Environments;
using Xunit;

namespace SOHTests.DomainTests.OutputTests;

public class TripsTests
{
    [Fact]
    public void AddFirstElement()
    {
        SimulationContext? context = SimulationContext.Start2020InSeconds;
        TripsCollection collection = new TripsCollection(context);
        collection.Add(1, Position.CreatePosition(0, 0));

        Assert.Single(collection.Result);
        (object? key, List<TripPosition>? tripPositions) = collection.Result.First();
        Assert.Equal(1, key);

        Assert.Single(tripPositions);
        Assert.Equal(0, tripPositions.First().X);
        Assert.Equal(0, tripPositions.First().Y);
    }

    [Fact]
    public void AddFirstElementWithoutKey()
    {
        SimulationContext? context = SimulationContext.Start2020InSeconds;
        TripsCollection collection = new TripsCollection(context);
        collection.Add(Position.CreatePosition(0, 0));

        Assert.Single(collection.Result);
        (object? key, List<TripPosition>? tripPositions) = collection.Result.First();
        Assert.Equal(0x01, key);

        Assert.Single(tripPositions);
        Assert.Equal(0, tripPositions.First().X);
        Assert.Equal(0, tripPositions.First().Y);
    }

    [Fact]
    public void AddElementToLastKey()
    {
        SimulationContext? context = SimulationContext.Start2020InSeconds;
        TripsCollection collection = new TripsCollection(context);
        collection.Add(0d, Position.CreatePosition(0, 0));
        collection.Add(Position.CreatePosition(1, 1));

        Assert.Single(collection.Result);
        (object? key, List<TripPosition>? tripPositions) = collection.Result.First();
        Assert.Equal(0d, key);

        Assert.Equal(0, tripPositions[0].X);
        Assert.Equal(0, tripPositions[0].Y);
        Assert.Equal(1, tripPositions[1].X);
        Assert.Equal(1, tripPositions[1].Y);
    }

    [Fact]
    public void AddSeveralElementsWithSameIdentifier()
    {
        SimulationContext? context = SimulationContext.Start2020InSeconds;
        TripsCollection collection = new TripsCollection(context);
        collection.Add(1, Position.CreatePosition(0, 0));
        collection.Add(1, Position.CreatePosition(1, 1));
        collection.Add(1, Position.CreatePosition(2, 2));

        Assert.Single(collection.Result);
        (object? key, List<TripPosition>? tripPositions) = collection.Result.First();
        Assert.Equal(1, key);

        Assert.Equal(3, tripPositions.Count);
        for (int i = 0; i < 3; i++)
        {
            Assert.Equal(i, tripPositions[i].X);
            Assert.Equal(i, tripPositions[i].Y);
        }
    }

    [Fact]
    public void AddElementsWithDifferentIdentifiers()
    {
        SimulationContext? context = SimulationContext.Start2020InSeconds;
        TripsCollection collection = new TripsCollection(context);
        collection.Add("1", Position.CreatePosition(0, 0));
        collection.Add("a", Position.CreatePosition(1, 1));
        collection.Add(ModalChoice.CyclingRentalBike, Position.CreatePosition(2, 2));
        collection.Add("a", Position.CreatePosition(3, 3));

        Assert.Equal(4, collection.Result.Count);

        int i = 0;
        foreach ((object, List<TripPosition>) tuple in collection.Result)
        {
            List<TripPosition> tripPositions = tuple.Item2;
            Assert.Single(tripPositions);
            Assert.Equal(i, tripPositions[0].X);
            Assert.Equal(i, tripPositions[0].Y);
            i++;
        }
    }

    [Fact]
    public void AddMultipleElementsWithDifferentIdentifiers()
    {
        SimulationContext? context = SimulationContext.Start2020InSeconds;
        TripsCollection collection = new TripsCollection(context);
        collection.Add(1, Position.CreatePosition(0, 0));
        collection.Add(1, Position.CreatePosition(1, 1));
        collection.Add("a", Position.CreatePosition(2, 2));
        collection.Add(1, Position.CreatePosition(3, 3));

        Assert.Equal(3, collection.Result.Count);

        (object? key0, List<TripPosition>? tripPositions0) = collection.Result[0];
        Assert.Equal(1, key0);
        Assert.Equal(0, tripPositions0[0].X);
        Assert.Equal(0, tripPositions0[0].Y);
        Assert.Equal(1, tripPositions0[1].X);
        Assert.Equal(1, tripPositions0[1].Y);

        (object? key1, List<TripPosition>? tripPositions1) = collection.Result[1];
        Assert.Equal("a", key1);
        Assert.Single(tripPositions1);
        Assert.Equal(2, tripPositions1[0].X);
        Assert.Equal(2, tripPositions1[0].Y);

        (object? key2, List<TripPosition>? tripPositions2) = collection.Result[2];
        Assert.Equal(1, key2);
        Assert.Single(tripPositions2);
        Assert.Equal(3, tripPositions2[0].X);
        Assert.Equal(3, tripPositions2[0].Y);
    }
}