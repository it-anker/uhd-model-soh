using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using MediatR;
using NetTopologySuite.Features;

namespace SOH.Process.Server.Models.Ogc;

public class GetJobResultRequest : IRequest<Results>
{
    [Required]
    [DataMember(Name = "jobId")]
    public string JobId { get; set; } = default!;
}

public class Result
{
    [Required] [DataMember(Name = "output")]
    public string Output { get; set; } = default!;

    [Required]
    [DataMember(Name = "processID")]
    public string ProcessId { get; set; } = default!;

    [Required] [DataMember(Name = "jobID")]
    public string JobId { get; set; } = default!;

    [DataMember(Name = "fileId")]
    public string? FileId { get; set; }

    [DataMember(Name = "featureCollection")]
    public FeatureCollection? FeatureCollection { get; set; }

    [Required]
    [DataMember(Name = "resultID")]
    public string Id { get; set; } = default!;
}

[DataContract]
public class Results : Dictionary<string, object>;