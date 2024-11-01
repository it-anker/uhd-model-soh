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

namespace SOHTests.SimulationTests.CarDrivingTests.DriveModeTwoTests;

[Collection("SimulationTests")]
public class RandomDestinationsAltona : IClassFixture<SpatialGraphFixture>
{
    private readonly SpatialGraphFixture _fixture;

    public RandomDestinationsAltona(SpatialGraphFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void RandomDestinationsAltonaTest()
    {
        LoggerFactory.SetLogLevel(LogLevel.Info);

        ModelDescription modelDescription = new ModelDescription();
        modelDescription.AddLayer<CarLayer>();
        modelDescription.AddLayer<TrafficLightLayer>();
        modelDescription.AddAgent<CarDriver, CarLayer>();
        modelDescription.AddEntity<Car>();

        DateTime startTime = DateTime.Parse("2020-01-01T00:00:00");
        SimulationConfig config = new SimulationConfig
        {
            Globals =
            {
                StartPoint = startTime,
                EndPoint = startTime + TimeSpan.FromMinutes(10),
                DeltaTUnit = TimeSpanUnit.Seconds,
                OutputTarget = OutputTargetType.Csv,
                CsvOptions =
                {
                    NumberFormat = "G",
                    Delimiter = ";",
                    OutputPath = GetType().Name
                }
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
                    InstanceCount = 10,
                    IndividualMapping = new List<IndividualMapping>
                    {
                        new() { Name = "driveMode", Value = 2 }
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

        Assert.Equal(600, workflowState.Iterations);

        //check that all agents have moved
        DataTable? table = CsvReader.MapData(Path.Combine(GetType().Name, nameof(CarDriver) + ".csv"));
        Assert.NotNull(table);

        Dictionary<string, Position> positionById = new Dictionary<string, Position>();
        DataRow[] firstTickRows = table.Select("Tick = '0'");
        foreach (DataRow row in firstTickRows)
        {
            string? id = row["ID"].Value<string>();
            Position? position =
                Position.CreateGeoPosition(row["Longitude"].Value<double>(), row["Latitude"].Value<double>());
            positionById.Add(id, position);
        }

        Assert.Equal(10, positionById.Count);

        DataRow[] lastTickRows = table.Select("Tick = '600'");
        foreach (DataRow row in lastTickRows)
        {
            string? id = row["ID"].Value<string>();
            Position? position =
                Position.CreateGeoPosition(row["Longitude"].Value<double>(), row["Latitude"].Value<double>());
            Assert.NotEqual(positionById[id], position);
        }
    }
}