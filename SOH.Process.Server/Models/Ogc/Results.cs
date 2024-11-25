using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace SOH.Process.Server.Models.Ogc;

public class Result : AbstractEntity
{
    [Required] [DataMember(Name = "simulationId")]
    public string SimulationId { get; set; } = default!;

    [Required]
    [DataMember(Name = "geoJson")]
    public string GeoJson { get; set; } = default!;
}

[DataContract]
public class Results : Dictionary<string, object>
{
    [DataMember(Name = "resultId")]
    public string? ResultId { get; set; }

    [Required]
    [DataMember(Name = "geoJson")]
    public string GeoJson { get; set; } = default!;
}