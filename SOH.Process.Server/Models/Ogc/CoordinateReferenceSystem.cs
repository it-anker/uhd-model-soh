using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SOH.Process.Server.Models.Ogc;

/// <summary>
///     Predefined coordinate reference system, used by the analyse.
/// </summary>
[JsonConverter(typeof(StringEnumConverter))]
public enum CoordinateReferenceSystem
{
    /// <summary>
    ///     Enum _13CRS84Enum for http://www.opengis.net/def/crs/OGC/1.3/CRS84
    /// </summary>
    [EnumMember(Value = "http://www.opengis.net/def/crs/OGC/1.3/CRS84")]
    Crs84 = 0,

    /// <summary>
    ///     Enum _0CRS84hEnum for http://www.opengis.net/def/crs/OGC/0/CRS84h
    /// </summary>
    [EnumMember(Value = "http://www.opengis.net/def/crs/OGC/0/CRS84h")]
    Crs84H = 1
}