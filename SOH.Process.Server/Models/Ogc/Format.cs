using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;

namespace SOH.Process.Server.Models.Ogc;

/// <summary>
/// </summary>
[DataContract]
public class Format : IEquatable<Format>
{
    /// <summary>
    ///     Gets or Sets MediaType
    /// </summary>

    [DataMember(Name = "mediaType")]
    public string MediaType { get; set; }

    /// <summary>
    ///     Gets or Sets Encoding
    /// </summary>

    [DataMember(Name = "encoding")]
    public string Encoding { get; set; }

    /// <summary>
    ///     Gets or Sets Schema
    /// </summary>

    [DataMember(Name = "schema")]
    public OneOfformatSchema Schema { get; set; }

    /// <summary>
    ///     Returns true if Format instances are equal
    /// </summary>
    /// <param name="other">Instance of Format to be compared</param>
    /// <returns>Boolean</returns>
    public bool Equals(Format other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;

        return
            (
                MediaType == other.MediaType ||
                (MediaType != null &&
                 MediaType.Equals(other.MediaType))
            ) &&
            (
                Encoding == other.Encoding ||
                (Encoding != null &&
                 Encoding.Equals(other.Encoding))
            ) &&
            (
                Schema == other.Schema ||
                (Schema != null &&
                 Schema.Equals(other.Schema))
            );
    }

    /// <summary>
    ///     Returns the string presentation of the object
    /// </summary>
    /// <returns>String presentation of the object</returns>
    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("class Format {\n");
        sb.Append("  MediaType: ").Append(MediaType).Append("\n");
        sb.Append("  Encoding: ").Append(Encoding).Append("\n");
        sb.Append("  Schema: ").Append(Schema).Append("\n");
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
        return obj.GetType() == GetType() && Equals((Format)obj);
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
            if (MediaType != null)
                hashCode = hashCode * 59 + MediaType.GetHashCode();
            if (Encoding != null)
                hashCode = hashCode * 59 + Encoding.GetHashCode();
            if (Schema != null)
                hashCode = hashCode * 59 + Schema.GetHashCode();
            return hashCode;
        }
    }

    #region Operators

#pragma warning disable 1591

    public static bool operator ==(Format left, Format right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(Format left, Format right)
    {
        return !Equals(left, right);
    }

#pragma warning restore 1591

    #endregion Operators
}