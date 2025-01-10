using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using SOH.Process.Server.Models.Processes;

namespace SOH.Process.Server.Models.Ogc;

[DataContract]
public class JobList
{
    /// <summary>
    ///     Gets or Sets Jobs.
    /// </summary>
    [Required]
    [DataMember(Name = "jobs")]
    public List<StatusInfo> Jobs { get; set; } = [];

    /// <summary>
    ///     Gets or Sets Links.
    /// </summary>
    [Required]
    [DataMember(Name = "links")]
    public List<Link> Links { get; set; } = [];

    public bool ShouldSerializeJobs() => Jobs != null! && Jobs.Count > 0;
    public bool ShouldSerializeLinks() => Links != null! && Links.Count > 0;
}