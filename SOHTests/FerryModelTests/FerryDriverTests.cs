using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mars.Common.IO.Csv;
using Mars.Components.Environments;
using Mars.Core.Data;
using Mars.Interfaces;
using Mars.Interfaces.Environments;
using Mars.Interfaces.Model;
using Mars.Interfaces.Model.Options;
using SOHModel.Ferry.Model;
using SOHModel.Ferry.Route;
using SOHModel.Ferry.Station;
using SOHTests.Commons.Layer;
using Xunit;

namespace SOHTests.FerryModelTests;

public class FerryDriverTests : IClassFixture<FerryRouteLayerFixture>
{
    private readonly FerryLayer _layer;
    private readonly FerryRouteLayerFixture _routeLayerFixture;

    public FerryDriverTests(FerryRouteLayerFixture routeLayerFixture)
    {
        _routeLayerFixture = routeLayerFixture;
        _layer = new FerryLayer(routeLayerFixture.FerryRouteLayer)
        {
            Context = SimulationContext.Start2020InSeconds,
            EntityManager = new EntityManagerImpl(CsvReader.MapData(ResourcesConstants.FerryCsv)),
            GraphEnvironment = new SpatialGraphEnvironment(new SpatialGraphOptions
            {
                GraphImports = new List<Input>
                {
                    new()
                    {
                        File = ResourcesConstants.FerryGraph,
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
    public void VisitAllStationsAlongFerryRoute()
    {
        const int line = 61;
        FerryDriver driver = new FerryDriver(_layer, (_, _) => { })
        {
            Line = line,
            MinimumBoardingTimeInSeconds = 10
        };

        Assert.True(_layer.FerryRouteLayer.FerryRoutes.TryGetValue(line, out FerryRoute? schedule));
        List<FerryRouteEntry> unvisitedStationEntries = schedule.Entries.ToList();
        Assert.Equal(8, unvisitedStationEntries.Count);
        for (int tick = 0; tick < 9000; tick++, _layer.Context.UpdateStep())
        {
            driver.Tick();
            if (driver.Boarding)
            {
                FerryRouteEntry routeEntry = driver.FerryRouteEnumerator.Current;
                unvisitedStationEntries.Remove(routeEntry);
            }
        }

        Assert.Empty(unvisitedStationEntries);
    }

    [Fact]
    public void SufficientBoardingTimeAtStations()
    {
        const int minimalBoardingTime = 32;

        FerryDriver driver = new FerryDriver(_layer, (_, _) => { })
        {
            Line = 61,
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

        FerryDriver driver = new FerryDriver(_layer, (_, _) => { })
        {
            Line = 62,
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

                FerryRouteEntry driverStationStops = driver.CurrentFerryRouteEntry;
                int travelTime = driverStationStops.Minutes * 60;
                nextStartTick += travelTime;
            }
        }
    }

    [Fact]
    public void FerryReachesReachStationsWithinDefinedStopMinutes()
    {
        FerryDriver driver = new FerryDriver(_layer, (_, _) => { })
        {
            Line = 62,
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
                    Assert.InRange(tick, firstDrivingTick + travelDurance, ticks);

                firstDrivingTick = -1;
            }
            else if (firstDrivingTick < 0)
            {
                firstDrivingTick = tick;
                travelDurance = driver.CurrentFerryRouteEntry.Minutes * 60 -
                                driver.MinimumBoardingTimeInSeconds * 2;
            }

            isFirstBoardingTick = !driver.Boarding;
        }
    }

    [Fact]
    public void TestMoveFerryAlongBidirectionalPath()
    {
        SpatialGraphEnvironment environment = new SpatialGraphEnvironment(new SpatialGraphOptions
        {
            GraphImports = new List<Input>
            {
                new()
                {
                    File = Path.Combine("res", "networks", "unidirected_line.geojson"),
                    InputConfiguration = new InputConfiguration
                    {
                        IsBiDirectedImport = true,
                        GeometryAsNodesEnabled = true
                    }
                }
            }
        });

        EntityManagerImpl manager = new EntityManagerImpl(CsvReader.MapData(ResourcesConstants.FerryCsv));

        FerryLayer layer = new FerryLayer(_routeLayerFixture.FerryRouteLayer)
        {
            EntityManager = manager,
            GraphEnvironment = environment,
            Context = SimulationContext.Start2020InSeconds
        };

        Route p1 = environment.FindShortestRoute(environment.Nodes.Last(), environment.Nodes.First());
        Route p2 = environment.FindShortestRoute(environment.Nodes.First(), environment.Nodes.Last());
        Assert.Equal(8, environment.Nodes.Count);
        Assert.Equal(14, environment.Edges.Count);
        Assert.Equal(p1.Count, p2.Count);
        Assert.Equal(p1.RemainingRouteDistanceToGoal, p2.RemainingRouteDistanceToGoal, 6);
        Position? source = environment.Nodes.First().Position;
        Position? target = environment.Nodes.Last().Position;

        Route route = environment.FindShortestRoute(environment.NearestNode(source),
            environment.NearestNode(target));

        Assert.NotEmpty(route);
        bool goalReached = false;

        FerryDriver driver = new FerryDriver(layer, (_, _) => goalReached = true)
        {
            Line = 61,
            Position = source,
            FerryRoute = new FerryRoute
            {
                Entries = route.Select(stop => new FerryRouteEntry(
                    new FerryStation
                    {
                        Position = stop.Edge.From.Position
                    }, new FerryStation
                    {
                        Position = stop.Edge.To.Position
                    }, 0)).ToList()
            }
        };

        Assert.NotNull(driver.Ferry);
        Assert.NotNull(driver.Layer);
        Assert.NotEqual(Guid.Empty, driver.ID);
        for (int i = 0; i < 10000; i++, layer.Context.UpdateStep()) driver.Tick();
        Assert.True(goalReached);
        Assert.True(driver.StationStops > 0);
        Assert.True(driver.GoalReached);
        Assert.NotEqual(source, driver.Position);
    }
}