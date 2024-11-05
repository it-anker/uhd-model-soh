using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SOH.Process.Server.Models.Ogc;

namespace SOH.Process.Server.Models.Processes;

/// <summary>
///     Gets or Sets Type.
/// </summary>
[JsonConverter(typeof(StringEnumConverter))]
public enum ProcessingKind
{
    /// <summary>
    ///     Enum ProcessEnum for process
    /// </summary>
    [EnumMember(Value = "process")] ProcessEnum = 0
}

[DataContract]
public sealed class StatusInfo : IEquatable<StatusInfo>
{
    /// <summary>
    ///     Gets or Sets ProcessID.
    /// </summary>
    [DataMember(Name = "processID")]
    public string? ProcessId { get; set; }

    /// <summary>
    ///     Gets or Sets Type.
    /// </summary>
    [DataMember(Name = "type")]
    public ProcessingKind Type { get; set; } = ProcessingKind.ProcessEnum;

    /// <summary>
    ///     Gets or Sets JobID.
    /// </summary>
    [Required]
    [DataMember(Name = "jobID")]
    public string JobId { get; set; } = default!;

    /// <summary>
    ///     Gets or Sets Status.
    /// </summary>
    [Required]
    [DataMember(Name = "status")]
    public StatusCode Status { get; set; }

    /// <summary>
    ///     Gets or Sets Message.
    /// </summary>
    [DataMember(Name = "message")]
    public string? Message { get; set; }

    /// <summary>
    ///     Gets or Sets Created.
    /// </summary>
    [DataMember(Name = "created")]
    public DateTime? Created { get; set; }

    /// <summary>
    ///     Gets or Sets Started.
    /// </summary>
    [DataMember(Name = "started")]
    public DateTime? Started { get; set; }

    /// <summary>
    ///     Gets or Sets Finished.
    /// </summary>
    [DataMember(Name = "finished")]
    public DateTime? Finished { get; set; }

    /// <summary>
    ///     Gets or Sets Updated.
    /// </summary>
    [DataMember(Name = "updated")]
    public DateTime? Updated { get; set; }

    /// <summary>
    ///     Gets or Sets Progress.
    /// </summary>
    [Range(0, 100)]
    [DataMember(Name = "progress")]
    public int? Progress { get; set; }

    /// <summary>
    ///     Gets or Sets Links.
    /// </summary>
    [DataMember(Name = "links")]
    public List<Link> Links { get; set; } = [];

    public bool Equals(StatusInfo? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return ProcessId == other.ProcessId && Type == other.Type &&
               JobId == other.JobId && Status == other.Status;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((StatusInfo)obj);
    }

    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add(ProcessId);
        hashCode.Add(Type);
        hashCode.Add(JobId);
        hashCode.Add((int)Status);
        hashCode.Add(Message);
        hashCode.Add(Created);
        hashCode.Add(Started);
        hashCode.Add(Finished);
        hashCode.Add(Updated);
        hashCode.Add(Progress);
        hashCode.Add(Links);
        return hashCode.ToHashCode();
    }
}