using System.Runtime.Serialization;
using Newtonsoft.Json.Converters;

namespace SOH.Process.Server.Models.Ogc;

/// <summary>
///     The kind, how the process shall be executed,
///     processing completely in blocking mode
///     or async in background.
/// </summary>
[Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
public enum JobControlOptions
{
    /// <summary>
    ///     Enum SyncExecuteEnum for sync-execute
    /// </summary>
    [EnumMember(Value = "sync-execute")] SynchronousExecution = 0,

    /// <summary>
    ///     Enum AsyncExecuteEnum for async-execute
    /// </summary>
    [EnumMember(Value = "async-execute")] AsyncExecution = 1,

    /// <summary>
    ///     Enum DismissEnum for dismiss
    /// </summary>
    [EnumMember(Value = "dismiss")] Dismiss = 2
}