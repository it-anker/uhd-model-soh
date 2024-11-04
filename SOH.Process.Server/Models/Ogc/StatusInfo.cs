using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SOH.Process.Server.Models.Ogc;

/// <summary>
/// </summary>
[DataContract]
public class StatusInfo : IEquatable<StatusInfo>
{
    /// <summary>
    ///     Gets or Sets Type
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum TypeEnum
    {
        /// <summary>
        ///     Enum ProcessEnum for process
        /// </summary>
        [EnumMember(Value = "process")] ProcessEnum = 0
    }

    /// <summary>
    ///     Gets or Sets ProcessID
    /// </summary>

    [DataMember(Name = "processID")]
    public string ProcessID { get; set; }

    /// <summary>
    ///     Gets or Sets Type
    /// </summary>
    [Required]
    [DataMember(Name = "type")]
    public TypeEnum? Type { get; set; }

    /// <summary>
    ///     Gets or Sets JobID
    /// </summary>
    [Required]
    [DataMember(Name = "jobID")]
    public string JobID { get; set; }

    /// <summary>
    ///     Gets or Sets Status
    /// </summary>
    [Required]
    [DataMember(Name = "status")]
    public StatusCode Status { get; set; }

    /// <summary>
    ///     Gets or Sets Message
    /// </summary>

    [DataMember(Name = "message")]
    public string Message { get; set; }

    /// <summary>
    ///     Gets or Sets Created
    /// </summary>

    [DataMember(Name = "created")]
    public DateTime? Created { get; set; }

    /// <summary>
    ///     Gets or Sets Started
    /// </summary>

    [DataMember(Name = "started")]
    public DateTime? Started { get; set; }

    /// <summary>
    ///     Gets or Sets Finished
    /// </summary>

    [DataMember(Name = "finished")]
    public DateTime? Finished { get; set; }

    /// <summary>
    ///     Gets or Sets Updated
    /// </summary>

    [DataMember(Name = "updated")]
    public DateTime? Updated { get; set; }

    /// <summary>
    ///     Gets or Sets Progress
    /// </summary>

    [Range(0, 100)]
    [DataMember(Name = "progress")]
    public int? Progress { get; set; }

    /// <summary>
    ///     Gets or Sets Links
    /// </summary>

    [DataMember(Name = "links")]
    public List<Link> Links { get; set; }

    /// <summary>
    ///     Returns true if StatusInfo instances are equal
    /// </summary>
    /// <param name="other">Instance of StatusInfo to be compared</param>
    /// <returns>Boolean</returns>
    public bool Equals(StatusInfo other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;

        return
            (
                ProcessID == other.ProcessID ||
                (ProcessID != null &&
                 ProcessID.Equals(other.ProcessID))
            ) &&
            (
                Type == other.Type ||
                (Type != null &&
                 Type.Equals(other.Type))
            ) &&
            (
                JobID == other.JobID ||
                (JobID != null &&
                 JobID.Equals(other.JobID))
            ) &&
            (
                Status == other.Status ||
                (Status != null &&
                 Status.Equals(other.Status))
            ) &&
            (
                Message == other.Message ||
                (Message != null &&
                 Message.Equals(other.Message))
            ) &&
            (
                Created == other.Created ||
                (Created != null &&
                 Created.Equals(other.Created))
            ) &&
            (
                Started == other.Started ||
                (Started != null &&
                 Started.Equals(other.Started))
            ) &&
            (
                Finished == other.Finished ||
                (Finished != null &&
                 Finished.Equals(other.Finished))
            ) &&
            (
                Updated == other.Updated ||
                (Updated != null &&
                 Updated.Equals(other.Updated))
            ) &&
            (
                Progress == other.Progress ||
                (Progress != null &&
                 Progress.Equals(other.Progress))
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
        sb.Append("class StatusInfo {\n");
        sb.Append("  ProcessID: ").Append(ProcessID).Append("\n");
        sb.Append("  Type: ").Append(Type).Append("\n");
        sb.Append("  JobID: ").Append(JobID).Append("\n");
        sb.Append("  Status: ").Append(Status).Append("\n");
        sb.Append("  Message: ").Append(Message).Append("\n");
        sb.Append("  Created: ").Append(Created).Append("\n");
        sb.Append("  Started: ").Append(Started).Append("\n");
        sb.Append("  Finished: ").Append(Finished).Append("\n");
        sb.Append("  Updated: ").Append(Updated).Append("\n");
        sb.Append("  Progress: ").Append(Progress).Append("\n");
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
        return obj.GetType() == GetType() && Equals((StatusInfo)obj);
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
            if (ProcessID != null)
                hashCode = hashCode * 59 + ProcessID.GetHashCode();
            if (Type != null)
                hashCode = hashCode * 59 + Type.GetHashCode();
            if (JobID != null)
                hashCode = hashCode * 59 + JobID.GetHashCode();
            if (Status != null)
                hashCode = hashCode * 59 + Status.GetHashCode();
            if (Message != null)
                hashCode = hashCode * 59 + Message.GetHashCode();
            if (Created != null)
                hashCode = hashCode * 59 + Created.GetHashCode();
            if (Started != null)
                hashCode = hashCode * 59 + Started.GetHashCode();
            if (Finished != null)
                hashCode = hashCode * 59 + Finished.GetHashCode();
            if (Updated != null)
                hashCode = hashCode * 59 + Updated.GetHashCode();
            if (Progress != null)
                hashCode = hashCode * 59 + Progress.GetHashCode();
            if (Links != null)
                hashCode = hashCode * 59 + Links.GetHashCode();
            return hashCode;
        }
    }

    #region Operators

#pragma warning disable 1591

    public static bool operator ==(StatusInfo left, StatusInfo right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(StatusInfo left, StatusInfo right)
    {
        return !Equals(left, right);
    }

#pragma warning restore 1591

    #endregion Operators
}