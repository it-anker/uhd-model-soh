using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using SOH.Process.Server.Models.Processes;

namespace SOH.Process.Server.Models.Ogc;

[DataContract]
public sealed class JobList : IEquatable<JobList>
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

    public bool Equals(JobList? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Jobs.Equals(other.Jobs) && Links.Equals(other.Links);
    }

    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj) || (obj is JobList other && Equals(other));
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Jobs, Links);
    }
}