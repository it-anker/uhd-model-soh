using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using SOH.Process.Server.Models.Ogc;

namespace SOH.Process.Server.Models.Parameters;

[DataContract]
public sealed class Bbox : IEquatable<Bbox>
{
    /// <summary>
    ///     Gets or Sets _Bbox.
    /// </summary>
    [Required]
    [DataMember(Name = "bbox")]
    public List<decimal?> _Bbox { get; set; } = [];

    /// <summary>
    ///     Gets or Sets Crs.
    /// </summary>
    [DataMember(Name = "crs")]
    public CoordinateReferenceSystem? Crs { get; set; }

    public bool Equals(Bbox? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return _Bbox.Equals(other._Bbox) && Crs == other.Crs;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((Bbox)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_Bbox, Crs);
    }
}