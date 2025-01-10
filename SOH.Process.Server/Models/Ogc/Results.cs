using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using MediatR;
using NetTopologySuite.Features;
using Swashbuckle.AspNetCore.Annotations;

namespace SOH.Process.Server.Models.Ogc;

public class GetJobResultRequest : IRequest<JobResultResponse>
{
    [Required]
    [DataMember(Name = "jobId")]
    public string JobId { get; set; } = null!;
}

public class JobResultResponse
{
    public Results? DocumentOutput { get; set; }

    public object? RawSingleOutput { get; set; }

    public List<object?>? RawMultiOutput { get; set; }

    public List<string>? RawReferences { get; set; }
}

public class TimeSeriesResult
{
    [DataMember(Name = "reference")]
    public string? Reference { get; set; }

    [DataMember(Name = "steps")]
    public List<TimeSeriesStep> Steps { get; set; } = [];

    public bool ShouldSerializeSteps() => Steps != null! && Steps.Count > 0;
}

public class TimeSeriesStep
{
    [DataMember(Name = "dateTime")]
    public DateTime? DateTime { get; set; }

    [DataMember(Name = "tick")]
    public long Tick { get; set; }

    [DataMember(Name = "value"), Required]
    public double Value { get; set; }
}

public class Result
{
    [Required]
    [DataMember(Name = "processID")]
    public string ProcessId { get; set; } = null!;

    [Required] [DataMember(Name = "jobID")]
    public string JobId { get; set; } = null!;

    [DataMember(Name = "fileId")]
    public string? FileId { get; set; }

    [DataMember(Name = "data")]
    public Dictionary<string, ResultEntry> Results { get; set; } = [];

    [Required]
    [DataMember(Name = "resultID")]
    public string Id { get; set; } = null!;

    public bool ShouldSerializeResults() => Results != null! && Results.Count > 0;
}

public class ResultEntry
{
    [DataMember(Name = "featureCollection")]
    public FeatureCollection? FeatureCollection { get; set; }

    [DataMember(Name = "timeSeries")]
    public List<TimeSeriesStep>? TimeSeries { get; set; }

    [DataMember(Name = "value")]
    public object? Value { get; set; }

    public object? GetValue()
    {
        return FeatureCollection ?? TimeSeries ?? Value;
    }
}

[DataContract]
public class Results : Dictionary<string, object?>
{
    /// <summary>
    ///     The links to all results by reference.
    /// </summary>
    [DataMember(Name = "links")]
    public List<Link> Links { get; set; } = [];

    public bool ShouldSerializeLinks() => Links != null! && Links.Count > 0;
}