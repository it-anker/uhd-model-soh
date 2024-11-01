using System;
using System.Collections.Generic;
using Mars.Common.Core.Logging;
using Mars.Components.Environments;
using Mars.Components.Starter;
using Mars.Core.Simulation.Entities;
using Mars.Interfaces.Model;
using SOHModel.Car.Model;
using SOHTests.SimulationTests.CarDrivingTests.IntersectionBehaviorTests;
using Xunit;

namespace SOHTests.SimulationTests.CarDrivingTests.DriveModeOneTests;

[Collection("SimulationTests")]
public class DriveRandomAltona : IClassFixture<SpatialGraphFixture>
{
    private readonly SpatialGraphFixture _fixture;

    public DriveRandomAltona(SpatialGraphFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void DriveRandomAltonaTest()
    {
        LoggerFactory.SetLogLevel(LogLevel.Info);

        ModelDescription modelDescription = new ModelDescription();
        modelDescription.AddLayer<CarLayer>();
        modelDescription.AddAgent<CarDriver, CarLayer>();
        modelDescription.AddEntity<Car>();

        DateTime start = DateTime.Parse("2020-01-01T00:00:00");
        SimulationConfig config = new SimulationConfig
        {
            Globals =
            {
                StartPoint = start,
                EndPoint = start + TimeSpan.FromMinutes(10),
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
                }
            },
            AgentMappings = new List<AgentMapping>
            {
                new()
                {
                    Name = nameof(CarDriver),
                    InstanceCount = 40,
                    IndividualMapping = new List<IndividualMapping>
                    {
                        new() { Name = "driveMode", Value = 1 }
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
    }
}