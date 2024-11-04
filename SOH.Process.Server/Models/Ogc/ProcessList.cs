using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;

namespace SOH.Process.Server.Models.Ogc;

/// <summary>
/// </summary>
[DataContract]
public class ProcessList : IEquatable<ProcessList>
{
    /// <summary>
    ///     Gets or Sets Processes
    /// </summary>
    [Required]
    [DataMember(Name = "processes")]
    public List<ProcessSummary> Processes { get; set; }

    /// <summary>
    ///     Gets or Sets Links
    /// </summary>
    [Required]
    [DataMember(Name = "links")]
    public List<Link> Links { get; set; }

    /// <summary>
    ///     Returns true if ProcessList instances are equal
    /// </summary>
    /// <param name="other">Instance of ProcessList to be compared</param>
    /// <returns>Boolean</returns>
    public bool Equals(ProcessList other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;

        return
            (
                Processes == other.Processes ||
                (Processes != null &&
                 Processes.SequenceEqual(other.Processes))
            ) &&
            (
                Links == other.Links ||
                (Links != null &&
                 Links.SequenceEqual(other.Links))
            );
    }

    /// <summary>
    ///     Returns the string presentation of the object
    /// </summary>
    /// <returns>String presentation of the object</returns>
    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("class ProcessList {\n");
        sb.Append("  Processes: ").Append(Processes).Append("\n");
        sb.Append("  Links: ").Append(Links).Append("\n");
        sb.Append("}\n");
        return sb.ToString();
    }

    /// <summary>
    ///     Returns the JSON string presentation of the object
    /// </summary>
    /// <returns>JSON string presentation of the object</returns>
    public string ToJson()
    {
        return JsonConvert.SerializeObject(this, Formatting.Indented);
    }

    /// <summary>
    ///     Returns true if objects are equal
    /// </summary>
    /// <param name="obj">Object to be compared</param>
    /// <returns>Boolean</returns>
    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((ProcessList)obj);
    }

    /// <summary>
    ///     Gets the hash code
    /// </summary>
    /// <returns>Hash code</returns>
    public override int GetHashCode()
    {
        unchecked // Overflow is fine, just wrap
        {
            int hashCode = 41;
            // Suitable nullity checks etc, of course :)
            if (Processes != null)
                hashCode = hashCode * 59 + Processes.GetHashCode();
            if (Links != null)
                hashCode = hashCode * 59 + Links.GetHashCode();
            return hashCode;
        }
    }

    #region Operators

#pragma warning disable 1591

    public static bool operator ==(ProcessList left, ProcessList right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(ProcessList left, ProcessList right)
    {
        return !Equals(left, right);
    }

#pragma warning restore 1591

    #endregion Operators
}