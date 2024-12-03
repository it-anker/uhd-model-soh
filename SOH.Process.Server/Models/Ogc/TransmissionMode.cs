using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SOH.Process.Server.Models.Ogc;

/// <summary>
///     The kind, how result shall be transmitted - as providing only the reference or the concrete value.
/// </summary>
[JsonConverter(typeof(StringEnumConverter))]
public enum TransmissionMode
{
    /// <summary>
    ///     The concrete value is transmitted.
    /// </summary>
    [EnumMember(Value = "value")] Value = 0,

    /// <summary>
    ///     The reference to get the value is transmitted.
    /// </summary>
    [EnumMember(Value = "reference")] Reference = 1
}