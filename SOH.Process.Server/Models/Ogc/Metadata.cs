using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;

namespace SOH.Process.Server.Models.Ogc;

/// <summary>
/// </summary>
[DataContract]
public class Metadata : IEquatable<Metadata>
{
    /// <summary>
    ///     Gets or Sets Title
    /// </summary>

    [DataMember(Name = "title")]
    public string Title { get; set; }

    /// <summary>
    ///     Gets or Sets Role
    /// </summary>

    [DataMember(Name = "role")]
    public string Role { get; set; }

    /// <summary>
    ///     Gets or Sets Href
    /// </summary>

    [DataMember(Name = "href")]
    public string Href { get; set; }

    /// <summary>
    ///     Returns true if Metadata instances are equal
    /// </summary>
    /// <param name="other">Instance of Metadata to be compared</param>
    /// <returns>Boolean</returns>
    public bool Equals(Metadata other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;

        return
            (
                Title == other.Title ||
                (Title != null &&
                 Title.Equals(other.Title))
            ) &&
            (
                Role == other.Role ||
                (Role != null &&
                 Role.Equals(other.Role))
            ) &&
            (
                Href == other.Href ||
                (Href != null &&
                 Href.Equals(other.Href))
            );
    }

    /// <summary>
    ///     Returns the string presentation of the object
    /// </summary>
    /// <returns>String presentation of the object</returns>
    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("class Metadata {\n");
        sb.Append("  Title: ").Append(Title).Append("\n");
        sb.Append("  Role: ").Append(Role).Append("\n");
        sb.Append("  Href: ").Append(Href).Append("\n");
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
        return obj.GetType() == GetType() && Equals((Metadata)obj);
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
            if (Title != null)
                hashCode = hashCode * 59 + Title.GetHashCode();
            if (Role != null)
                hashCode = hashCode * 59 + Role.GetHashCode();
            if (Href != null)
                hashCode = hashCode * 59 + Href.GetHashCode();
            return hashCode;
        }
    }

    #region Operators

#pragma warning disable 1591

    public static bool operator ==(Metadata left, Metadata right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(Metadata left, Metadata right)
    {
        return !Equals(left, right);
    }

#pragma warning restore 1591

    #endregion Operators
}