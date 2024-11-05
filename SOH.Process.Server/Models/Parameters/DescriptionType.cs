using System.Runtime.Serialization;
using SOH.Process.Server.Models.Ogc;

namespace SOH.Process.Server.Models.Parameters;

[DataContract]
public class DescriptionType : IEquatable<DescriptionType>
{
    /// <summary>
    ///     Gets or Sets Title.
    /// </summary>
    [DataMember(Name = "title")]
    public string? Title { get; set; }

    /// <summary>
    ///     Gets or Sets Description.
    /// </summary>
    [DataMember(Name = "description")]
    public string? Description { get; set; }

    /// <summary>
    ///     Gets or Sets Keywords.
    /// </summary>
    [DataMember(Name = "keywords")]
    public List<string> Keywords { get; set; } = [];

    /// <summary>
    ///     Gets or Sets Metadata.
    /// </summary>
    [DataMember(Name = "metadata")]
    public List<Metadata> Metadata { get; set; } = [];

    /// <summary>
    ///     Gets or Sets AdditionalParameters.
    /// </summary>
    [DataMember(Name = "additionalParameters")]
    public AllOfdescriptionTypeAdditionalParameters? AdditionalParameters { get; set; }

    public bool Equals(DescriptionType? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Title == other.Title && Description == other.Description &&
               Keywords.Equals(other.Keywords) && Metadata.Equals(other.Metadata) &&
               Equals(AdditionalParameters, other.AdditionalParameters);
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((DescriptionType)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Title, Description, Keywords, Metadata, AdditionalParameters);
    }
}