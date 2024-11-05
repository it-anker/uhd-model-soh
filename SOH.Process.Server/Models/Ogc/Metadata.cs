using System.Runtime.Serialization;

namespace SOH.Process.Server.Models.Ogc;

[DataContract]
public class Metadata : IEquatable<Metadata>
{
    /// <summary>
    ///     Gets or Sets Title.
    /// </summary>
    [DataMember(Name = "title")]
    public string? Title { get; set; }

    /// <summary>
    ///     Gets or Sets Role.
    /// </summary>
    [DataMember(Name = "role")]
    public string? Role { get; set; }

    /// <summary>
    ///     Gets or Sets Href.
    /// </summary>
    [DataMember(Name = "href")]
    public string? Href { get; set; }

    public bool Equals(Metadata? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Title == other.Title && Role == other.Role && Href == other.Href;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((Metadata)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Title, Role, Href);
    }
}