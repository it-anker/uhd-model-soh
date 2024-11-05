using System.Runtime.Serialization;

namespace SOH.Process.Server.Models.Parameters;

[DataContract]
public class Format : IEquatable<Format>
{
    /// <summary>
    ///     Gets or Sets MediaType.
    /// </summary>
    [DataMember(Name = "mediaType")]
    public string? MediaType { get; set; }

    /// <summary>
    ///     Gets or Sets Encoding.
    /// </summary>
    [DataMember(Name = "encoding")]
    public string? Encoding { get; set; }

    /// <summary>
    ///     Gets or Sets Schema.
    /// </summary>
    [DataMember(Name = "schema")]
    public object? Schema { get; set; }

    public bool Equals(Format? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return MediaType == other.MediaType && Encoding == other.Encoding && Equals(Schema, other.Schema);
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((Format)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(MediaType, Encoding, Schema);
    }
}