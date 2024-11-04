using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SOH.Process.Server.Models.Ogc;

/// <summary>
/// </summary>
[DataContract]
public class Bbox : IEquatable<Bbox>, InputValueNoObject
{
    /// <summary>
    ///     Gets or Sets Crs
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum CrsEnum
    {
        /// <summary>
        ///     Enum _13CRS84Enum for http://www.opengis.net/def/crs/OGC/1.3/CRS84
        /// </summary>
        [EnumMember(Value = "http://www.opengis.net/def/crs/OGC/1.3/CRS84")]
        _13CRS84Enum = 0,

        /// <summary>
        ///     Enum _0CRS84hEnum for http://www.opengis.net/def/crs/OGC/0/CRS84h
        /// </summary>
        [EnumMember(Value = "http://www.opengis.net/def/crs/OGC/0/CRS84h")]
        _0CRS84hEnum = 1
    }

    /// <summary>
    ///     Gets or Sets _Bbox
    /// </summary>
    [Required]
    [DataMember(Name = "bbox")]
    public List<decimal?> _Bbox { get; set; }

    /// <summary>
    ///     Gets or Sets Crs
    /// </summary>

    [DataMember(Name = "crs")]
    public CrsEnum? Crs { get; set; }

    /// <summary>
    ///     Returns true if Bbox instances are equal
    /// </summary>
    /// <param name="other">Instance of Bbox to be compared</param>
    /// <returns>Boolean</returns>
    public bool Equals(Bbox other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;

        return
            (
                _Bbox == other._Bbox ||
                (_Bbox != null &&
                 _Bbox.SequenceEqual(other._Bbox))
            ) &&
            (
                Crs == other.Crs ||
                (Crs != null &&
                 Crs.Equals(other.Crs))
            );
    }

    /// <summary>
    ///     Returns the string presentation of the object
    /// </summary>
    /// <returns>String presentation of the object</returns>
    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("class Bbox {\n");
        sb.Append("  _Bbox: ").Append(_Bbox).Append("\n");
        sb.Append("  Crs: ").Append(Crs).Append("\n");
        sb.Append("}\n");
        return sb.ToString();
    }

    /// <summary>
    ///     Returns the JSON string presentation of the object
    /// </summary>
    /// <returns>JSON string presentation of the object</returns>
    public string ToJson()
    {
        return JsonConvert.SerializeObject(this, Formatting.Indented);
    }

    /// <summary>
    ///     Returns true if objects are equal
    /// </summary>
    /// <param name="obj">Object to be compared</param>
    /// <returns>Boolean</returns>
    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((Bbox)obj);
    }

    /// <summary>
    ///     Gets the hash code
    /// </summary>
    /// <returns>Hash code</returns>
    public override int GetHashCode()
    {
        unchecked // Overflow is fine, just wrap
        {
            int hashCode = 41;
            // Suitable nullity checks etc, of course :)
            if (_Bbox != null)
                hashCode = hashCode * 59 + _Bbox.GetHashCode();
            if (Crs != null)
                hashCode = hashCode * 59 + Crs.GetHashCode();
            return hashCode;
        }
    }

    #region Operators

#pragma warning disable 1591

    public static bool operator ==(Bbox left, Bbox right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(Bbox left, Bbox right)
    {
        return !Equals(left, right);
    }

#pragma warning restore 1591

    #endregion Operators
}