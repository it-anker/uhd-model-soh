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
public class StatusInfo
{
    /// <summary>
    ///     Gets or Sets JobID.
    /// </summary>
    [Required]
    [DataMember(Name = "jobID")]
    public string JobId { get; set; } = default!;

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
    ///     The time in UTC when this job was created.
    /// </summary>
    [DataMember(Name = "created")]
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;

    /// <summary>
    ///     The time in UTC when this job started the analysis.
    /// </summary>
    [DataMember(Name = "started")]
    public DateTime? StartedUtc { get; set; }

    /// <summary>
    ///     The time in UTC when this job finished the analysis.
    /// </summary>
    [DataMember(Name = "finished")]
    public DateTime? FinishedUtc { get; set; }

    /// <summary>
    ///     The last modification time in UTC of this job.
    /// </summary>
    [DataMember(Name = "updated")]
    public DateTime UpdatedUtc { get; set; } = DateTime.UtcNow;

    /// <summary>
    ///     The actual progress in percentage (0-100).
    /// </summary>
    [Range(0, 100)]
    [DataMember(Name = "progress")]
    public int? Progress { get; set; }

    /// <summary>
    ///     Any additional links of this job.
    /// </summary>
    [DataMember(Name = "links")]
    public List<Link> Links { get; set; } = [];
}