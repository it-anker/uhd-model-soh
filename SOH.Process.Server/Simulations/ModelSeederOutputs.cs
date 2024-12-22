using Mars.Interfaces.Environments;
using Microsoft.Extensions.Localization;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using SOH.Process.Server.Models.Ogc;
using SOH.Process.Server.Models.Parameters;
using SOH.Process.Server.Resources;

namespace SOH.Process.Server.Simulations;

public static class ModelSeederOutputs
{
    public static Dictionary<string, OutputDescription> GetSmartOpenHamburgOutputs(
        IStringLocalizer<SharedResource> localization)
    {
        return new Dictionary<string, OutputDescription>
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
                "soh_output_avg_road_count", new OutputDescription
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
                            new() { Tick = 1, DateTime = DateTime.Today.AddHours(10), Value = 3.123 },
                            new() { Tick = 2, DateTime = DateTime.Today.AddHours(10).AddSeconds(1), Value = 3.314 },
                            new() { Tick = 3, DateTime = DateTime.Today.AddHours(10).AddSeconds(2), Value = 3.458 },
                            new() { Tick = 4, DateTime = DateTime.Today.AddHours(10).AddSeconds(3), Value = 3.513 }
                        }
                    }
                }
            },
            {
                "soh_output_sum_modality_usage", new OutputDescription
                {
                    Format = new Format
                    {
                        MediaType = "application/json",
                    },
                    Schema = new Schema
                    {
                        Title = localization["soh_output_sum_modality_usage"],
                        ContentMediaType = "application/json",
                        Description = localization["soh_output_sum_modality_usage_desc"],
                        Default = new Dictionary<string, List<TimeSeriesStep>>
                        {
                            {
                                nameof(SpatialModalityType.Walking), [
                                    new TimeSeriesStep
                                        { Tick = 1, DateTime = DateTime.Today.AddHours(10), Value = 3400 },
                                    new TimeSeriesStep
                                    {
                                        Tick = 2, DateTime = DateTime.Today.AddHours(10).AddSeconds(1), Value = 3431
                                    },
                                    new TimeSeriesStep
                                    {
                                        Tick = 3, DateTime = DateTime.Today.AddHours(10).AddSeconds(2), Value = 3438
                                    },
                                    new TimeSeriesStep
                                    {
                                        Tick = 4, DateTime = DateTime.Today.AddHours(10).AddSeconds(3), Value = 3442
                                    }
                                ]
                            },
                            {
                                nameof(SpatialModalityType.CarDriving), [
                                    new TimeSeriesStep
                                        { Tick = 1, DateTime = DateTime.Today.AddHours(10), Value = 1031 },
                                    new TimeSeriesStep
                                    {
                                        Tick = 2, DateTime = DateTime.Today.AddHours(10).AddSeconds(1), Value = 1037
                                    },
                                    new TimeSeriesStep
                                    {
                                        Tick = 3, DateTime = DateTime.Today.AddHours(10).AddSeconds(2), Value = 1037
                                    },
                                    new TimeSeriesStep
                                    {
                                        Tick = 4, DateTime = DateTime.Today.AddHours(10).AddSeconds(3), Value = 1037
                                    }
                                ]
                            }
                        }
                    }
                }
            },
        };
    }
}