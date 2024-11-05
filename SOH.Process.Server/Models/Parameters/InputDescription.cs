using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using SOH.Process.Server.Models.Ogc;

namespace SOH.Process.Server.Models.Parameters;

[DataContract]
public sealed class InputDescription : DescriptionType, IEquatable<InputDescription>
{
    /// <summary>
    ///     Gets or Sets MinOccurs.
    /// </summary>
    [DataMember(Name = "minOccurs")]
    public int? MinOccurs { get; set; }

    /// <summary>
    ///     Gets or Sets MaxOccurs.
    /// </summary>
    [DataMember(Name = "maxOccurs")]
    public object? MaxOccurs { get; set; }

    /// <summary>
    ///     Gets or Sets Schema.
    /// </summary>
    [Required]
    [DataMember(Name = "schema")]
    public Schema Schema { get; set; } = default!;

    public bool Equals(InputDescription? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return base.Equals(other) && MinOccurs == other.MinOccurs &&
               Equals(MaxOccurs, other.MaxOccurs) && Schema.Equals(other.Schema);
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((InputDescription)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), MinOccurs, MaxOccurs, Schema);
    }
}