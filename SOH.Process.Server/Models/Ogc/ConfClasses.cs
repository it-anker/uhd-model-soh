using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace SOH.Process.Server.Models.Ogc;

[DataContract]
public sealed class ConfClasses : IEquatable<ConfClasses>
{
    /// <summary>
    ///     Gets or Sets ConformsTo.
    /// </summary>
    [Required]
    [DataMember(Name = "conformsTo")]
    public List<string> ConformsTo { get; set; } = [];

    public bool Equals(ConfClasses? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return ConformsTo.Equals(other.ConformsTo);
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((ConfClasses)obj);
    }

    public override int GetHashCode()
    {
        return ConformsTo.GetHashCode();
    }
}