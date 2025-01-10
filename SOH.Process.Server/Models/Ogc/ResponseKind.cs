using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SOH.Process.Server.Models.Ogc;

/// <summary>
///     The response kind enabled for result.
/// </summary>
[JsonConverter(typeof(StringEnumConverter))]
public enum ResponseKind
{
    /// <summary>
    ///     Enum RawEnum for raw
    /// </summary>
    [EnumMember(Value = "raw")] RawEnum = 0,

    /// <summary>
    ///     Enum DocumentEnum for document
    /// </summary>
    [EnumMember(Value = "document")] Document = 1
}