using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace SOH.Process.Server.Models.Parameters;

[DataContract]
public sealed class QualifiedInputValue : Format, IEquatable<QualifiedInputValue>
{
    /// <summary>
    ///     Gets or Sets Value.
    /// </summary>
    [Required]
    [DataMember(Name = "value")]
    public object Value { get; set; } = default!;

    public bool Equals(QualifiedInputValue? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return base.Equals(other) && Value.Equals(other.Value);
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((QualifiedInputValue)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), Value);
    }
}