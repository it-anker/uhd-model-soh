using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;

namespace SOH.Process.Server.Models.Ogc;

/// <summary>
/// </summary>
[DataContract]
public class ProcessSummary : DescriptionType, IEquatable<ProcessSummary>
{
    /// <summary>
    ///     Gets or Sets Id
    /// </summary>
    [Required]
    [DataMember(Name = "id")]
    public string Id { get; set; }

    /// <summary>
    ///     Gets or Sets Version
    /// </summary>
    [Required]
    [DataMember(Name = "version")]
    public string Version { get; set; }

    /// <summary>
    ///     Gets or Sets JobControlOptions
    /// </summary>

    [DataMember(Name = "jobControlOptions")]
    public List<JobControlOptions> JobControlOptions { get; set; }

    /// <summary>
    ///     Gets or Sets OutputTransmission
    /// </summary>

    [DataMember(Name = "outputTransmission")]
    public List<TransmissionMode> OutputTransmission { get; set; }

    /// <summary>
    ///     Gets or Sets Links
    /// </summary>

    [DataMember(Name = "links")]
    public List<Link> Links { get; set; }

    /// <summary>
    ///     Returns true if ProcessSummary instances are equal
    /// </summary>
    /// <param name="other">Instance of ProcessSummary to be compared</param>
    /// <returns>Boolean</returns>
    public bool Equals(ProcessSummary other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;

        return
            (
                Id == other.Id ||
                (Id != null &&
                 Id.Equals(other.Id))
            ) &&
            (
                Version == other.Version ||
                (Version != null &&
                 Version.Equals(other.Version))
            ) &&
            (
                JobControlOptions == other.JobControlOptions ||
                (JobControlOptions != null &&
                 JobControlOptions.SequenceEqual(other.JobControlOptions))
            ) &&
            (
                OutputTransmission == other.OutputTransmission ||
                (OutputTransmission != null &&
                 OutputTransmission.SequenceEqual(other.OutputTransmission))
            ) &&
            (
                Links == other.Links ||
                (Links != null &&
                 Links.SequenceEqual(other.Links))
            ) && base.Equals(other);
    }

    /// <summary>
    ///     Returns the string presentation of the object
    /// </summary>
    /// <returns>String presentation of the object</returns>
    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("class ProcessSummary {\n");
        sb.Append("  Id: ").Append(Id).Append("\n");
        sb.Append("  Version: ").Append(Version).Append("\n");
        sb.Append("  JobControlOptions: ").Append(JobControlOptions).Append("\n");
        sb.Append("  OutputTransmission: ").Append(OutputTransmission).Append("\n");
        sb.Append("  Links: ").Append(Links).Append("\n");
        sb.Append("}\n");
        return sb.ToString();
    }

    /// <summary>
    ///     Returns the JSON string presentation of the object
    /// </summary>
    /// <returns>JSON string presentation of the object</returns>
    public new string ToJson()
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
        return obj.GetType() == GetType() && Equals((ProcessSummary)obj);
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
            if (Id != null)
                hashCode = hashCode * 59 + Id.GetHashCode();
            if (Version != null)
                hashCode = hashCode * 59 + Version.GetHashCode();
            if (JobControlOptions != null)
                hashCode = hashCode * 59 + JobControlOptions.GetHashCode();
            if (OutputTransmission != null)
                hashCode = hashCode * 59 + OutputTransmission.GetHashCode();
            if (Links != null)
                hashCode = hashCode * 59 + Links.GetHashCode();
            return hashCode;
        }
    }

    #region Operators

#pragma warning disable 1591

    public static bool operator ==(ProcessSummary left, ProcessSummary right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(ProcessSummary left, ProcessSummary right)
    {
        return !Equals(left, right);
    }

#pragma warning restore 1591

    #endregion Operators
}