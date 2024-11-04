using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;

namespace SOH.Process.Server.Models.Ogc;

/// <summary>
/// </summary>
[DataContract]
public class InputDescription : DescriptionType, IEquatable<InputDescription>
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
    public Schema? Schema { get; set; }

    /// <summary>
    ///     Returns true if InputDescription instances are equal
    /// </summary>
    /// <param name="other">Instance of InputDescription to be compared</param>
    /// <returns>Boolean</returns>
    public bool Equals(InputDescription other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;

        return
            (MinOccurs == other.MinOccurs) &&
            (MaxOccurs == other.MaxOccurs) &&
            Schema != null &&
            Schema.Equals(other.Schema) && base.Equals(other);
    }

    /// <summary>
    ///     Returns true if objects are equal
    /// </summary>
    /// <param name="obj">Object to be compared</param>
    /// <returns>Boolean</returns>
    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((InputDescription)obj);
    }

    /// <summary>
    ///     Gets the hash code
    /// </summary>
    /// <returns>Hash code</returns>
    public override int GetHashCode()
    {
        unchecked
        {
            int hashCode = 41;
            // Suitable nullity checks etc, of course :)
            if (MinOccurs != null)
                hashCode = hashCode * 59 + MinOccurs.GetHashCode();
            if (MaxOccurs != null)
                hashCode = hashCode * 59 + MaxOccurs.GetHashCode();
            if (Schema != null)
                hashCode = hashCode * 59 + Schema.GetHashCode();
            return hashCode;
        }
    }
}