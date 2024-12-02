using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using NetTopologySuite.Features;

namespace SOH.Process.Server.Models.Ogc;

public class Result : AbstractEntity
{
    [Required]
    [DataMember(Name = "processID")]
    public string SimulationId { get; set; } = default!;

    [Required] [DataMember(Name = "jobID")]
    public string JobId { get; set; } = default!;

    [DataMember(Name = "fileId")]
    public string? FileId { get; set; }

    [DataMember(Name = "featureCollection")]
    public FeatureCollection? FeatureCollection { get; set; }

    [Required]
    [DataMember(Name = "resultID")]
    public override string Id { get; set; } = default!;
}

[DataContract]
public class Results : Dictionary<string, object>;