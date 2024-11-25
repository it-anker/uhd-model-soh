using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace SOH.Process.Server.Models.Ogc;

public abstract class AbstractEntity
{
    [Required] [DataMember(Name = "id")]
    public string Id { get; set; } = default!;

    public bool Equals(AbstractEntity? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Id == other.Id;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((AbstractEntity)obj);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}