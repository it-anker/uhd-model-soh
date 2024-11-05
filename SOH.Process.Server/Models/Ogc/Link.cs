using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using SOH.Process.Server.Models.Common;

namespace SOH.Process.Server.Models.Ogc;

[DataContract]
public sealed class Link : IEquatable<Link>
{
    /// <summary>
    ///     Gets or Sets Href.
    /// </summary>
    [Required]
    [DataMember(Name = "href")]
    public string Href { get; set; } = default!;

    /// <summary>
    ///     Gets or Sets Rel.
    /// </summary>
    [DataMember(Name = "rel")]
    public string? Rel { get; set; }

    /// <summary>
    ///     Gets or Sets Type.
    /// </summary>
    [DataMember(Name = "type")]
    public string? Type { get; set; }

    /// <summary>
    ///     Gets or Sets Hreflang.
    /// </summary>
    [DataMember(Name = "hreflang")]
    public string? Hreflang { get; set; }

    /// <summary>
    ///     Gets or Sets Title.
    /// </summary>
    [DataMember(Name = "title")]
    public string? Title { get; set; }

    public bool Equals(Link? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Href == other.Href && Rel == other.Rel && Type == other.Type && Hreflang == other.Hreflang && Title == other.Title;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((Link)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Href, Rel, Type, Hreflang, Title);
    }
}