using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;

namespace SOH.Process.Server.Models.Ogc;

/// <summary>
///     
/// </summary>
[DataContract]
public class JobList : IEquatable<JobList>
{
    /// <summary>
    ///     Gets or Sets Jobs
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

    /// <summary>
    ///     Returns the string presentation of the object.
    /// </summary>
    /// <returns>String presentation of the object.</returns>
    public override string ToString()
    {
        StringBuilder sb = new ();
        sb.Append("class JobList {\n");
        sb.Append("  Jobs: ").Append(Jobs).Append("\n");
        sb.Append("  Links: ").Append(Links).Append("\n");
        sb.Append("}\n");
        return sb.ToString();
    }

    /// <summary>
    ///     Returns true if JobList instances are equal
    /// </summary>
    public bool Equals(JobList? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;

        return (Jobs == other.Jobs || Jobs.SequenceEqual(other.Jobs)) &&
               (Links == other.Links || Links.SequenceEqual(other.Links));
    }

    /// <summary>
    ///     Returns true if objects are equal
    /// </summary>
    /// <param name="obj">Object to be compared</param>
    /// <returns>Boolean</returns>
    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((JobList)obj);
    }

    /// <summary>
    ///     Gets the hash code
    /// </summary>
    /// <returns>Hash code</returns>
    public override int GetHashCode()
    {
        unchecked
        {
            int hashCode = 41;
            hashCode = hashCode * 59 + Jobs.GetHashCode();
            hashCode = hashCode * 59 + Links.GetHashCode();
            return hashCode;
        }
    }
}