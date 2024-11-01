using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Mars.Common.Core;
using Mars.Common.Core.Logging;
using Mars.Common.IO.Csv;
using Mars.Components.Environments;
using Mars.Components.Starter;
using Mars.Core.Simulation.Entities;
using Mars.Interfaces.Environments;
using Mars.Interfaces.Model;
using SOHModel.Car.Model;
using SOHModel.Multimodal.Layers.TrafficLight;
using SOHTests.SimulationTests.CarDrivingTests.IntersectionBehaviorTests;
using Xunit;

namespace SOHTests.SimulationTests.CarDrivingTests.DriveModeThreeTests;

[Collection("SimulationTests")]
public class DriveFromAtoBWithLights : IClassFixture<SpatialGraphFixture>
{
    private readonly SpatialGraphFixture _fixture;

    public DriveFromAtoBWithLights(SpatialGraphFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void DriveFromAtoB()
    {
        LoggerFactory.SetLogLevel(LogLevel.Warning);

        ModelDescription modelDescription = new ModelDescription();

        modelDescription.AddLayer<TrafficLightLayer>();
        modelDescription.AddLayer<CarLayer>();
        modelDescription.AddAgent<CarDriver, CarLayer>();
        modelDescription.AddEntity<Car>();

        DateTime startTime = DateTime.Parse("2020-01-01T00:00:00");
        Position? start = Position.CreateGeoPosition(9.937944800, 53.547771400);
        Position? goal = Position.CreateGeoPosition(9.948982100, 53.552160201);
        SimulationConfig config = new SimulationConfig
        {
            Globals =
            {
                StartPoint = startTime,
                EndPoint = startTime + TimeSpan.FromHours(1),
                DeltaTUnit = TimeSpanUnit.Seconds,
                OutputTarget = OutputTargetType.Csv,
                CsvOptions = { OutputPath = GetType().Name }
            },
            LayerMappings =
            {
                new LayerMapping
                {
                    Name = nameof(CarLayer),
                    Value = new SpatialGraphEnvironment(_fixture.DriveGraphAltonaAltstadt)
                },
                new LayerMapping
                {
                    Name = nameof(TrafficLightLayer),
                    File = ResourcesConstants.TrafficLightsAltona
                }
            },
            AgentMappings = new List<AgentMapping>
            {
                new()
                {
                    Name = nameof(CarDriver),
                    InstanceCount = 1,
                    IndividualMapping = new List<IndividualMapping>
                    {
                        new() { Name = "driveMode", Value = 3 },
                        new() { Name = "startLat", Value = start.Latitude },
                        new() { Name = "startLon", Value = start.Longitude },
                        new() { Name = "destLat", Value = goal.Latitude },
                        new() { Name = "destLon", Value = goal.Longitude }
                    }
                }
            },
            EntityMappings = new List<EntityMapping>
            {
                new()
                {
                    Name = nameof(Car),
                    File = ResourcesConstants.CarCsv
                }
            }
        };

        SimulationStarter starter = SimulationStarter.Start(modelDescription, config);
        SimulationWorkflowState workflowState = starter.Run();

        Assert.Equal(3600, workflowState.Iterations);

        DataTable? table = CsvReader.MapData(Path.Combine(GetType().Name, nameof(CarDriver) + ".csv"));
        Assert.NotNull(table);

        DataRow firstRow = table.Select("Tick = '0'")[0];
        Position? posAfterFirstTick =
            Position.CreateGeoPosition(firstRow["Longitude"].Value<double>(), firstRow["Latitude"].Value<double>());
        Assert.InRange(posAfterFirstTick.DistanceInMTo(start), 0, 5);

        DataRow lastRow = table.Select("Tick = '276'")[0];
        Assert.Equal("True", lastRow["GoalReached"]);
        Assert.InRange(lastRow["RemainingRouteDistanceToGoal"].Value<double>(), -0.1, 0.1);
        Position? reachedGoal =
            Position.CreateGeoPosition(lastRow["Longitude"].Value<double>(), lastRow["Latitude"].Value<double>());
        Assert.InRange(reachedGoal.DistanceInMTo(goal), 0, 3);
    }
}