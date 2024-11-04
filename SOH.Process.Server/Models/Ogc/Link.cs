using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;

namespace SOH.Process.Server.Models.Ogc;

/// <summary>
/// </summary>
[DataContract]
public class Link : IEquatable<Link>, InlineOrRefData
{
    /// <summary>
    ///     Gets or Sets Href
    /// </summary>
    [Required]
    [DataMember(Name = "href")]
    public string Href { get; set; } = default!;

    /// <summary>
    ///     Gets or Sets Rel.
    /// </summary>

    [DataMember(Name = "rel")]
    public string Rel { get; set; }

    /// <summary>
    ///     Gets or Sets Type
    /// </summary>

    [DataMember(Name = "type")]
    public string Type { get; set; }

    /// <summary>
    ///     Gets or Sets Hreflang
    /// </summary>

    [DataMember(Name = "hreflang")]
    public string Hreflang { get; set; }

    /// <summary>
    ///     Gets or Sets Title
    /// </summary>

    [DataMember(Name = "title")]
    public string Title { get; set; }

    /// <summary>
    ///     Returns true if Link instances are equal
    /// </summary>
    /// <param name="other">Instance of Link to be compared</param>
    /// <returns>Boolean</returns>
    public bool Equals(Link other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;

        return
            (
                Href == other.Href ||
                (Href != null &&
                 Href.Equals(other.Href))
            ) &&
            (
                Rel == other.Rel ||
                (Rel != null &&
                 Rel.Equals(other.Rel))
            ) &&
            (
                Type == other.Type ||
                (Type != null &&
                 Type.Equals(other.Type))
            ) &&
            (
                Hreflang == other.Hreflang ||
                (Hreflang != null &&
                 Hreflang.Equals(other.Hreflang))
            ) &&
            (
                Title == other.Title ||
                (Title != null &&
                 Title.Equals(other.Title))
            );
    }

    /// <summary>
    ///     Returns the string presentation of the object
    /// </summary>
    /// <returns>String presentation of the object</returns>
    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("class Link {\n");
        sb.Append("  Href: ").Append(Href).Append("\n");
        sb.Append("  Rel: ").Append(Rel).Append("\n");
        sb.Append("  Type: ").Append(Type).Append("\n");
        sb.Append("  Hreflang: ").Append(Hreflang).Append("\n");
        sb.Append("  Title: ").Append(Title).Append("\n");
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
        return obj.GetType() == GetType() && Equals((Link)obj);
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
            if (Href != null)
                hashCode = hashCode * 59 + Href.GetHashCode();
            if (Rel != null)
                hashCode = hashCode * 59 + Rel.GetHashCode();
            if (Type != null)
                hashCode = hashCode * 59 + Type.GetHashCode();
            if (Hreflang != null)
                hashCode = hashCode * 59 + Hreflang.GetHashCode();
            if (Title != null)
                hashCode = hashCode * 59 + Title.GetHashCode();
            return hashCode;
        }
    }

    #region Operators

#pragma warning disable 1591

    public static bool operator ==(Link left, Link right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(Link left, Link right)
    {
        return !Equals(left, right);
    }

#pragma warning restore 1591

    #endregion Operators
}