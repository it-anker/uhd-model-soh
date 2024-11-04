using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SOH.Process.Server.Models.Ogc;

/// <summary>
///     Gets or Sets jobControlOptions
/// </summary>
[JsonConverter(typeof(StringEnumConverter))]
public enum JobControlOptions
{
    /// <summary>
    ///     Enum SyncExecuteEnum for sync-execute
    /// </summary>
    [EnumMember(Value = "sync-execute")] SyncExecuteEnum = 0,

    /// <summary>
    ///     Enum AsyncExecuteEnum for async-execute
    /// </summary>
    [EnumMember(Value = "async-execute")] AsyncExecuteEnum = 1,

    /// <summary>
    ///     Enum DismissEnum for dismiss
    /// </summary>
    [EnumMember(Value = "dismiss")] DismissEnum = 2
}