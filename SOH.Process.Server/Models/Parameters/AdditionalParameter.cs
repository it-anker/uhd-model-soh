using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace SOH.Process.Server.Models.Parameters;

[DataContract]
public sealed class AdditionalParameter : IEquatable<AdditionalParameter>
{
    /// <summary>
    ///     Gets or Sets Name.
    /// </summary>
    [Required]
    [DataMember(Name = "name")]
    public string Name { get; set; } = default!;

    /// <summary>
    ///     Gets or Sets Value.
    /// </summary>
    [Required]
    [DataMember(Name = "value")]
    public List<object> Value { get; set; } = [];

    /// <summary>
    ///     Returns true if AdditionalParameter instances are equal.
    /// </summary>
    /// <param name="other">Instance of AdditionalParameter to be compared</param>
    /// <returns>Boolean</returns>
    public bool Equals(AdditionalParameter? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;

        return Name == other.Name &&
               Value.SequenceEqual(other.Value);
    }

    /// <summary>
    ///     Returns true if objects are equal.
    /// </summary>
    /// <param name="obj">Object to be compared.</param>
    /// <returns>Boolean</returns>
    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((AdditionalParameter)obj);
    }
}