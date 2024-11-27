using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SOH.Process.Server.Models.Processes;

/// <summary>
///     Gets or Sets statusCode.
/// </summary>
[JsonConverter(typeof(StringEnumConverter))]
public enum StatusCode
{
    /// <summary>
    ///     The job status when this job was accepted and scheduled but not yet started.
    /// </summary>
    [EnumMember(Value = "accepted")] Accepted = 0,

    /// <summary>
    ///     The job status when this job is currently running the selected process.
    /// </summary>
    [EnumMember(Value = "running")] Running = 1,

    /// <summary>
    ///     The job status when this job was successful after running the selected process.
    /// </summary>
    [EnumMember(Value = "successful")] Successful = 2,

    /// <summary>
    ///     The job status when this job was not successful and a problem occured during
    ///     execution of the selected process.
    /// </summary>
    [EnumMember(Value = "failed")] Failed = 3,

    /// <summary>
    ///     The job status when this job was created but rejected due to invalid input.
    /// </summary>
    [EnumMember(Value = "dismissed")] Dismissed = 4
}