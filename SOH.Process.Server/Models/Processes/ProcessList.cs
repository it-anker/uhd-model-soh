using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using SOH.Process.Server.Models.Ogc;

namespace SOH.Process.Server.Models.Processes;

[DataContract]
public sealed class ProcessList : IEquatable<ProcessList>
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

    public bool Equals(ProcessList? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Processes.Equals(other.Processes) && Links.Equals(other.Links);
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((ProcessList)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Processes, Links);
    }
}