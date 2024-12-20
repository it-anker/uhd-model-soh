using Mapster;
using Microsoft.Extensions.Localization;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using SOH.Process.Server.Models.Ogc;
using SOH.Process.Server.Models.Parameters;
using SOH.Process.Server.Resources;

namespace SOH.Process.Server.Simulations;

internal class ModelSeeder(
    ISimulationService simulationService,
    IConfiguration configuration,
    IStringLocalizer<SharedResource> localization) : ICustomSeeder
{
    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        await SeedFerrySimulationProcess(cancellationToken);
        await SeedGreen4BikesSimulationProcess(cancellationToken);
#if DEBUG

        var testModel = new CreateSimulationProcessDescriptionRequest
        {
            ExecutionKind = ProcessExecutionKind.Direct,
            Title = "TestModel",
            Description = "TestDesc",
            Version = "0.0.1",
            Keywords = ["test"],
            IsTest = true,
            JobControlOptions =
            [
                JobControlOptions.SynchronousExecution
            ],
            OutputTransmission =
            [
                TransmissionMode.Value
            ],
            Outputs = new Dictionary<string, OutputDescription>
            {
                {
                    "default", new OutputDescription
                    {
                        Format = new Format
                        {
                            MediaType = "application/geo+json"
                        },
                        Schema = new Schema
                        {
                            Title = "Default test output",
                            ContentMediaType = "application/geo+json",
                            Default = new FeatureCollection()
                        }
                    }
                }
            }
        };

        const string testId = "simulation:testProcessId";
        var testProcess = await simulationService.FindSimulationAsync(testId, cancellationToken);
        if (testProcess == null)
        {
            await simulationService.CreateAsync(testId, testModel, cancellationToken);
        }
#endif
    }

    private async Task SeedFerrySimulationProcess(CancellationToken cancellationToken)
    {
        string host = configuration["ApiBaseUrl"] !;
        if (!host.EndsWith('/')) host += "/";

        var ferryModel = new CreateSimulationProcessDescriptionRequest
        {
            ExecutionKind = ProcessExecutionKind.Direct,
            Title = localization["ferry_transfer_model_title"],
            Description = localization["ferry_transfer_model_description"],
            Version = "1.0.0",
            Keywords = ["ferry", "transfer", "simulation"],
            JobControlOptions =
            [
                JobControlOptions.SynchronousExecution,
                JobControlOptions.AsyncExecution
            ],
            OutputTransmission =
            [
                TransmissionMode.Value
            ],
            Inputs = new Dictionary<string, InputDescription>
            {
                {
                    "startPoint", new InputDescription
                    {
                        Title = "The start time point of the simulation.",
                        Description = "The time point when the simulation starts internally. " +
                                      "Specified as ISO 8601 format",
                        Schema = new Schema
                        {
                            Type = "string",
                            Format = "dateTime",
                            Nullable = true
                        }
                    }
                },
                {
                    "endPoint", new InputDescription
                    {
                        Title = "The end time point of the simulation.",
                        Description = "The time point when the simulation end internally. " +
                                      "Specified as ISO 8601 format",
                        Schema = new Schema
                        {
                            Type = "string",
                            Format = "dateTime",
                            Nullable = true
                        }
                    }
                },
                {
                    "steps", new InputDescription
                    {
                        Title = "The amount of steps in seconds to simulate.",
                        Description = "The amount of steps in seconds to simulate used " +
                                      "instead of end time point, starting from start time point",
                        Schema = new Schema
                        {
                            Type = "number",
                            Maximum = 1000,
                            Minimum = 0
                        }
                    }
                }
            },
            Outputs = new Dictionary<string, OutputDescription>
            {
                {
                    "agents", new OutputDescription
                    {
                        Format = new Format
                        {
                            MediaType = "application/geo+json",
                        },
                        Schema = new Schema
                        {
                            Title = localization["soh_output_agents"],
                            ContentMediaType = "application/geo+json",
                            Default = new FeatureCollection(),
                            Example = new FeatureCollection
                            {
                                new Feature(new Point(9.978667786160287, 53.54407542750305),
                                    new AttributesTable
                                    {
                                        { "ActiveCapability", "Walking" },
                                        { "RouteLength", 8838 },
                                        { "DistanceStartGoal", 6281.220268477454 },
                                        { "tick", 12 },
                                        { "dateTime", "2024-12-01T07:20:01" }
                                    })
                            }
                        }
                    }
                },
                {
                    "avg_road_count", new OutputDescription
                    {
                        Format = new Format
                        {
                            MediaType = "application/json",
                        },
                        Schema = new Schema
                        {
                            Title = localization["soh_output_avg_road_count"],
                            ContentMediaType = "application/json",
                            Description = localization["soh_output_avg_road_count_desc"],
                            Default = new List<TimeSeriesStep>
                            {
                                new() { Tick = 1, DateTime = DateTime.Today, Value = 3.123 },
                                new() { Tick = 2, DateTime = DateTime.Today, Value = 3.314 },
                                new() { Tick = 3, DateTime = DateTime.Today, Value = 3.458 },
                                new() { Tick = 4, DateTime = DateTime.Today, Value = 3.513 }
                            }
                        }
                    }
                },
            },
            Links =
            [
                new Link
                {
                    Href = new Uri(host + $"/processes/{GlobalConstants.FerryTransferId}").ToString(),
                    Title = localization["model_self_description"],
                    Rel = "self",
                    Type = "application/json"
                }
            ]
        };

        var existingProcess = await simulationService.FindSimulationAsync(
            GlobalConstants.FerryTransferId, cancellationToken);

        if (existingProcess == null)
        {
            await simulationService.CreateAsync(GlobalConstants.FerryTransferId,
                ferryModel, cancellationToken);
        }
        else
        {
            var update = ferryModel.Adapt<UpdateSimulationProcessDescriptionRequest>();
            await simulationService.UpdateAsync(GlobalConstants.FerryTransferId,
                update, cancellationToken);
        }
    }

    private async Task SeedGreen4BikesSimulationProcess(CancellationToken cancellationToken)
    {
        string host = configuration["ApiBaseUrl"] !;
        if (!host.EndsWith('/')) host += "/";

        var ferryModel = new CreateSimulationProcessDescriptionRequest
        {
            ExecutionKind = ProcessExecutionKind.Direct,
            Title = localization["green_4_bikes_model_title"],
            Description = localization["green_4_bikes_model_description"],
            Version = "1.0.0",
            Keywords = ["cycling", "travelling", "simulation"],
            JobControlOptions =
            [
                JobControlOptions.SynchronousExecution,
                JobControlOptions.AsyncExecution
            ],
            OutputTransmission =
            [
                TransmissionMode.Value
            ],
            Inputs = new Dictionary<string, InputDescription>
            {
                {
                    "startPoint", new InputDescription
                    {
                        Title = "The start time point of the simulation.",
                        Description = "The time point when the simulation starts internally. " +
                                      "Specified as ISO 8601 format",
                        Schema = new Schema
                        {
                            Type = "string",
                            Format = "dateTime",
                            Nullable = true
                        }
                    }
                },
                {
                    "endPoint", new InputDescription
                    {
                        Title = "The end time point of the simulation.",
                        Description = "The time point when the simulation end internally. " +
                                      "Specified as ISO 8601 format",
                        Schema = new Schema
                        {
                            Type = "string",
                            Format = "dateTime",
                            Nullable = true
                        }
                    }
                },
                {
                    "steps", new InputDescription
                    {
                        Title = "The amount of steps in seconds to simulate.",
                        Description = "The amount of steps in seconds to simulate used " +
                                      "instead of end time point, starting from start time point",
                        Schema = new Schema
                        {
                            Type = "number",
                            Maximum = 1000,
                            Minimum = 0
                        }
                    }
                }
            },
            Outputs = new Dictionary<string, OutputDescription>
            {
                {
                    "agents", new OutputDescription
                    {
                        Format = new Format
                        {
                            MediaType = "application/geo+json",
                        },
                        Schema = new Schema
                        {
                            Title = localization["green_4_bikes_output_agents"],
                            ContentMediaType = "application/geo+json",
                            Default = new FeatureCollection(),
                            Example = new FeatureCollection
                            {
                                new Feature(new Point(9.978667786160287, 53.54407542750305),
                                    new AttributesTable
                                    {
                                        { "ActiveCapability", "Walking" },
                                        { "RouteLength", 8838 },
                                        { "DistanceStartGoal", 6281.220268477454 },
                                        { "tick", 12 },
                                        { "dateTime", "2024-12-01T07:20:01" }
                                    })
                            }
                        }
                    }
                },
                {
                    "avg_road_count", new OutputDescription
                    {
                        Format = new Format
                        {
                            MediaType = "application/json",
                        },
                        Schema = new Schema
                        {
                            Title = localization["soh_output_avg_road_count"],
                            ContentMediaType = "application/json",
                            Description = localization["soh_output_avg_road_count_desc"],
                            Default = new List<TimeSeriesStep>
                            {
                                new() { Tick = 1, DateTime = DateTime.Today, Value = 3.123 },
                                new() { Tick = 2, DateTime = DateTime.Today, Value = 3.314 },
                                new() { Tick = 3, DateTime = DateTime.Today, Value = 3.458 },
                                new() { Tick = 4, DateTime = DateTime.Today, Value = 3.513 }
                            }
                        }
                    }
                },
                {
                    "bicycleRentalStations", new OutputDescription
                    {
                        Format = new Format
                        {
                            MediaType = "application/geo+json",
                        },
                        Schema = new Schema
                        {
                            Title = localization["green_4_bikes_output_rental_stations"],
                            ContentMediaType = "application/geo+json",
                            Default = new FeatureCollection()
                        }
                    }
                }
            },
            Links =
            [
                new Link
                {
                    Href = new Uri(host + $"/processes/{GlobalConstants.FerryTransferId}").ToString(),
                    Title = localization["model_self_description"],
                    Rel = "self",
                    Type = "application/json"
                }
            ]
        };

        var existingProcess = await simulationService.FindSimulationAsync(
            GlobalConstants.Green4BikesId, cancellationToken);

        if (existingProcess == null)
        {
            await simulationService.CreateAsync(GlobalConstants.Green4BikesId,
                ferryModel, cancellationToken);
        }
        else
        {
            var update = ferryModel.Adapt<UpdateSimulationProcessDescriptionRequest>();
            await simulationService.UpdateAsync(GlobalConstants.Green4BikesId,
                update, cancellationToken);
        }
    }
}