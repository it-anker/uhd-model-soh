using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using SOH.Process.Server.Models.Parameters;

namespace SOH.Process.Server.Models.Ogc;

[DataContract]
public sealed class OutputDescription : DescriptionType, IEquatable<OutputDescription>
{
    [Required]
    [DataMember(Name = "schema")]
    public Schema Schema { get; set; } = default!;

    public bool Equals(OutputDescription? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return base.Equals(other) && Schema.Equals(other.Schema);
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((OutputDescription)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), Schema);
    }
}