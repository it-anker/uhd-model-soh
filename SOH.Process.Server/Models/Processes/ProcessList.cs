using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using SOH.Process.Server.Models.Ogc;

namespace SOH.Process.Server.Models.Processes;

[DataContract]
public sealed class ProcessList
{
    /// <summary>
    ///     Gets or Sets Processes.
    /// </summary>
    [Required]
    [DataMember(Name = "processes")]
    public List<ProcessSummary> Processes { get; set; } = [];

    /// <summary>
    ///     Gets or Sets Links.
    /// </summary>
    [Required]
    [DataMember(Name = "links")]
    public List<Link> Links { get; set; } = [];
}