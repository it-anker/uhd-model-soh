using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SOH.Process.Server.Models.Ogc;

/// <summary>
///     The kind, how result shall be transmitted - as providing only the referecne or the concrete value.
/// </summary>
[JsonConverter(typeof(StringEnumConverter))]
public enum TransmissionMode
{
    /// <summary>
    ///     Enum ValueEnum for value
    /// </summary>
    [EnumMember(Value = "value")] ValueEnum = 0,

    /// <summary>
    ///     Enum ReferenceEnum for reference
    /// </summary>
    [EnumMember(Value = "reference")] ReferenceEnum = 1
}