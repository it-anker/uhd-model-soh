using System;
using System.Collections.Generic;
using System.Linq;
using Mars.Common.IO.Csv;
using Mars.Components.Environments;
using Mars.Core.Data;
using Mars.Interfaces;
using Mars.Interfaces.Environments;
using Mars.Interfaces.Model;
using Mars.Interfaces.Model.Options;
using SOHModel.Train.Model;
using SOHModel.Train.Route;
using SOHModel.Train.Station;
using SOHTests.Commons.Layer;
using Xunit;

namespace SOHTests.TrainModelTests;

public class TrainDriverTests : IClassFixture<TrainRouteLayerFixture>
{
    private readonly TrainLayer _layer;
    private readonly TrainRouteLayerFixture _routeLayerFixture;

    public TrainDriverTests(TrainRouteLayerFixture routeLayerFixture)
    {
        _routeLayerFixture = routeLayerFixture;
        _layer = new TrainLayer(routeLayerFixture.TrainRouteLayer)
        {
            Context = SimulationContext.Start2020InSeconds,
            EntityManager = new EntityManagerImpl(CsvReader.MapData(ResourcesConstants.TrainCsv)),
            GraphEnvironment = new SpatialGraphEnvironment(new SpatialGraphOptions
            {
                GraphImports = new List<Input>
                {
                    new()
                    {
                        File = ResourcesConstants.TrainU1Graph,
                        InputConfiguration = new InputConfiguration
                        {
                            IsBiDirectedImport = true
                        }
                    }
                }
            })
        };
    }

    [Fact]
    public void VisitAllStationsAlongTrainRoute()
    {
        const string line = "U1";
        TrainDriver driver = new TrainDriver(_layer, (_, _) => { })
        {
            Line = line,
            MinimumBoardingTimeInSeconds = 10
        };

        Assert.True(_layer.TrainRouteLayer.TryGetRoute(line, out TrainRoute? schedule));
        List<TrainRouteEntry> unvisitedStationEntries = schedule.Entries.ToList();
        Assert.Equal(37, unvisitedStationEntries.Count);
        for (int tick = 0; tick < 10000; tick++, _layer.Context.UpdateStep())
        {
            driver.Tick();
            if (driver.Boarding)
            {
                TrainRouteEntry routeEntry = driver.TrainRouteEnumerator.Current;
                unvisitedStationEntries.Remove(routeEntry);
            }
        }

        Assert.Empty(unvisitedStationEntries);
    }

    [Fact]
    public void SufficientBoardingTimeAtStations()
    {
        const int minimalBoardingTime = 32;

        TrainDriver driver = new TrainDriver(_layer, (_, _) => { })
        {
            Line = "U1",
            MinimumBoardingTimeInSeconds = minimalBoardingTime
        };

        int currentBoardingTime = -1;
        for (int tick = 0; tick < 10000; tick++, _layer.Context.UpdateStep())
        {
            driver.Tick();

            if (driver.Boarding)
            {
                currentBoardingTime++;
            }
            else if (currentBoardingTime > 0)
            {
                Assert.InRange(currentBoardingTime, minimalBoardingTime, TimeSpan.FromMinutes(10).TotalSeconds);
                currentBoardingTime = -1;
            }
        }
    }

    [Fact]
    public void PunctualStartingTimeForNextRoute()
    {
        const int minimumBoardingTimeInSeconds = 10;

        TrainDriver driver = new TrainDriver(_layer, (_, _) => { })
        {
            Line = "U1",
            MinimumBoardingTimeInSeconds = minimumBoardingTimeInSeconds
        };

        bool firstTickAfterBoarding = false;

        int nextStartTick = minimumBoardingTimeInSeconds + 1;
        for (int tick = 0; tick < 2000; tick++, _layer.Context.UpdateStep())
        {
            driver.Tick();

            if (driver.Boarding)
            {
                firstTickAfterBoarding = true;
            }
            else if (firstTickAfterBoarding)
            {
                firstTickAfterBoarding = false;

                Assert.InRange(Math.Abs(tick - nextStartTick), 0, minimumBoardingTimeInSeconds * 2);

                TrainRouteEntry driverStationStops = driver.CurrentTrainRouteEntry;
                int travelTime = driverStationStops.Minutes * 60;
                nextStartTick += travelTime;
            }
        }
    }

    [Fact]
    public void TrainReachesReachStationsWithinDefinedStopMinutes()
    {
        TrainDriver driver = new TrainDriver(_layer, (_, _) => { })
        {
            Line = "U1",
            MinimumBoardingTimeInSeconds = 20
        };

        long firstDrivingTick = -1L;
        bool isFirstBoardingTick = true;
        int travelDurance = 0;

        const int ticks = 2000;
        for (int tick = 0; tick < ticks; tick++, _layer.Context.UpdateStep())
        {
            driver.Tick();

            if (driver.Boarding)
            {
                if (isFirstBoardingTick && firstDrivingTick >= 0)
                    Assert.InRange(tick, +travelDurance, ticks);

                firstDrivingTick = -1;
            }
            else if (firstDrivingTick < 0)
            {
                firstDrivingTick = tick;
                travelDurance = driver.CurrentTrainRouteEntry.Minutes * 60 -
                                driver.MinimumBoardingTimeInSeconds * 2;
            }

            isFirstBoardingTick = !driver.Boarding;
        }
    }

    [Fact]
    public void ImportU1NorthTrack()
    {
        SpatialGraphEnvironment environment = new SpatialGraphEnvironment(new SpatialGraphOptions
        {
            GraphImports = new List<Input>
            {
                new()
                {
                    File = ResourcesConstants.TrainU1NorthGraph,
                    InputConfiguration = new InputConfiguration
                    {
                        IsBiDirectedImport = true
                    }
                }
            }
        });
        Assert.Equal(10, environment.Nodes.Count);
        Assert.Equal(18, environment.Edges.Count);

        // all nodes are connected to the graph
        Assert.All(environment.Nodes, node => Assert.NotEmpty(node.OutgoingEdges));
    }

    [Fact]
    public void TestMoveTrainAlongBidirectionalPath()
    {
        SpatialGraphEnvironment environment = new SpatialGraphEnvironment(new SpatialGraphOptions
        {
            GraphImports = new List<Input>
            {
                new()
                {
                    File = ResourcesConstants.TrainU1NorthGraph,
                    InputConfiguration = new InputConfiguration
                    {
                        IsBiDirectedImport = true
                    }
                }
            }
        });

        EntityManagerImpl manager = new EntityManagerImpl(CsvReader.MapData(ResourcesConstants.TrainCsv));

        TrainLayer layer = new TrainLayer(_routeLayerFixture.TrainRouteLayer)
        {
            EntityManager = manager,
            GraphEnvironment = environment,
            Context = SimulationContext.Start2020InSeconds
        };

        Route p1 = environment.FindShortestRoute(environment.Nodes.Last(), environment.Nodes.First());
        Route p2 = environment.FindShortestRoute(environment.Nodes.First(), environment.Nodes.Last());
        Assert.Equal(p1.Count, p2.Count);
        Assert.Equal(p1.RemainingRouteDistanceToGoal, p2.RemainingRouteDistanceToGoal, 6);
        Position? source = environment.Nodes.First().Position;
        Position? target = environment.Nodes.Last().Position;

        Route route = environment.FindShortestRoute(environment.NearestNode(source),
            environment.NearestNode(target));

        Assert.NotEmpty(route);
        bool goalReached = false;

        TrainDriver driver = new TrainDriver(layer, (_, _) => goalReached = true)
        {
            Line = "U1",
            Position = source,
            TrainRoute = new TrainRoute
            {
                Entries = route.Select(stop => new TrainRouteEntry(
                    new TrainStation
                    {
                        Position = stop.Edge.From.Position
                    }, new TrainStation
                    {
                        Position = stop.Edge.To.Position
                    }, 0)).ToList()
            }
        };

        Assert.NotNull(driver.Train);
        Assert.NotNull(driver.Layer);
        Assert.NotEqual(Guid.Empty, driver.ID);
        for (int i = 0; i < 10000; i++, layer.Context.UpdateStep()) driver.Tick();
        Assert.True(goalReached);
        Assert.True(driver.StationStops > 0);
        Assert.True(driver.GoalReached);
        Assert.NotEqual(source, driver.Position);
    }

    [Fact]
    public void MoveAlongReveredTrainRouteTest()
    {
        TrainDriver driver = new TrainDriver(_layer, (_, _) => { })
        {
            Line = "U1",
            MinimumBoardingTimeInSeconds = 10,
            ReversedRoute = true
        };

        driver.Tick();

        TrainRouteEntry startTrainRouteEntry = driver.CurrentTrainRouteEntry;
        Assert.NotNull(startTrainRouteEntry);
        Assert.Equal("Ohlstedt", startTrainRouteEntry.From.Name);

        for (int tick = 0; tick < 10000; tick++, _layer.Context.UpdateStep()) driver.Tick();

        TrainRouteEntry goalTrainRouteEntry = driver.CurrentTrainRouteEntry;
        Assert.NotNull(goalTrainRouteEntry);
        Assert.Equal("Ochsenzoll", goalTrainRouteEntry.To.Name);
    }
}