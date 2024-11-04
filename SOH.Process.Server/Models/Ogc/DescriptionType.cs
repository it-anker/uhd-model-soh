using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;

namespace SOH.Process.Server.Models.Ogc;

/// <summary>
/// </summary>
[DataContract]
public class DescriptionType : IEquatable<DescriptionType>
{
    /// <summary>
    ///     Gets or Sets Title.
    /// </summary>

    [DataMember(Name = "title")]
    public string Title { get; set; } = default!;

    /// <summary>
    ///     Gets or Sets Description
    /// </summary>
    [DataMember(Name = "description")]
    public string Description { get; set; } = default!;

    /// <summary>
    ///     Gets or Sets Keywords.
    /// </summary>
    [DataMember(Name = "keywords")]
    public List<string> Keywords { get; set; } = default!;

    /// <summary>
    ///     Gets or Sets Metadata.
    /// </summary>
    [DataMember(Name = "metadata")]
    public List<Metadata> Metadata { get; set; } = default!;

    /// <summary>
    ///     Gets or Sets AdditionalParameters
    /// </summary>
    [DataMember(Name = "additionalParameters")]
    public AllOfdescriptionTypeAdditionalParameters AdditionalParameters { get; set; }

    /// <summary>
    ///     Returns true if DescriptionType instances are equal
    /// </summary>
    /// <param name="other">Instance of DescriptionType to be compared</param>
    /// <returns>Boolean</returns>
    public bool Equals(DescriptionType other)
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
                Description == other.Description ||
                (Description != null &&
                 Description.Equals(other.Description))
            ) &&
            (
                Keywords == other.Keywords ||
                (Keywords != null &&
                 Keywords.SequenceEqual(other.Keywords))
            ) &&
            (
                Metadata == other.Metadata ||
                (Metadata != null &&
                 Metadata.SequenceEqual(other.Metadata))
            ) &&
            (
                AdditionalParameters == other.AdditionalParameters ||
                (AdditionalParameters != null &&
                 AdditionalParameters.Equals(other.AdditionalParameters))
            );
    }

    /// <summary>
    ///     Returns the string presentation of the object
    /// </summary>
    /// <returns>String presentation of the object</returns>
    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("class DescriptionType {\n");
        sb.Append("  Title: ").Append(Title).Append("\n");
        sb.Append("  Description: ").Append(Description).Append("\n");
        sb.Append("  Keywords: ").Append(Keywords).Append("\n");
        sb.Append("  Metadata: ").Append(Metadata).Append("\n");
        sb.Append("  AdditionalParameters: ").Append(AdditionalParameters).Append("\n");
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
        return obj.GetType() == GetType() && Equals((DescriptionType)obj);
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
            if (Description != null)
                hashCode = hashCode * 59 + Description.GetHashCode();
            if (Keywords != null)
                hashCode = hashCode * 59 + Keywords.GetHashCode();
            if (Metadata != null)
                hashCode = hashCode * 59 + Metadata.GetHashCode();
            if (AdditionalParameters != null)
                hashCode = hashCode * 59 + AdditionalParameters.GetHashCode();
            return hashCode;
        }
    }

    #region Operators

#pragma warning disable 1591

    public static bool operator ==(DescriptionType left, DescriptionType right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(DescriptionType left, DescriptionType right)
    {
        return !Equals(left, right);
    }

#pragma warning restore 1591

    #endregion Operators
}