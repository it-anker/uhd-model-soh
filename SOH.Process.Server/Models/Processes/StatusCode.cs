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
    ///     Enum AcceptedEnum for accepted
    /// </summary>
    [EnumMember(Value = "accepted")] AcceptedEnum = 0,

    /// <summary>
    ///     Enum RunningEnum for running
    /// </summary>
    [EnumMember(Value = "running")] RunningEnum = 1,

    /// <summary>
    ///     Enum SuccessfulEnum for successful
    /// </summary>
    [EnumMember(Value = "successful")] SuccessfulEnum = 2,

    /// <summary>
    ///     Enum FailedEnum for failed
    /// </summary>
    [EnumMember(Value = "failed")] FailedEnum = 3,

    /// <summary>
    ///     Enum DismissedEnum for dismissed
    /// </summary>
    [EnumMember(Value = "dismissed")] DismissedEnum = 4
}