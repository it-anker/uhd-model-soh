using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;

namespace SOH.Process.Server.Models.Ogc;

/// <summary>
/// </summary>
[DataContract]
public class ConfClasses : IEquatable<ConfClasses>
{
    /// <summary>
    ///     Gets or Sets ConformsTo
    /// </summary>
    [Required]
    [DataMember(Name = "conformsTo")]
    public List<string> ConformsTo { get; set; } = [];

    /// <summary>
    ///     Returns true if ConfClasses instances are equal
    /// </summary>
    /// <param name="other">Instance of ConfClasses to be compared</param>
    /// <returns>Boolean</returns>
    public bool Equals(ConfClasses other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;

        return
            ConformsTo == other.ConformsTo ||
            (ConformsTo != null &&
             ConformsTo.SequenceEqual(other.ConformsTo));
    }

    /// <summary>
    ///     Returns the string presentation of the object
    /// </summary>
    /// <returns>String presentation of the object</returns>
    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("class ConfClasses {\n");
        sb.Append("  ConformsTo: ").Append(ConformsTo).Append("\n");
        sb.Append("}\n");
        return sb.ToString();
    }

    /// <summary>
    ///     Returns the JSON string presentation of the object
    /// </summary>
    /// <returns>JSON string presentation of the object</returns>
    public string ToJson()
    {
        return JsonConvert.SerializeObject(this, Formatting.Indented);
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
        return obj.GetType() == GetType() && Equals((ConfClasses)obj);
    }

    /// <summary>
    ///     Gets the hash code.
    /// </summary>
    /// <returns>Hash code</returns>
    public override int GetHashCode()
    {
        unchecked // Overflow is fine, just wrap
        {
            int hashCode = 41;
            // Suitable nullity checks etc, of course :)
            if (ConformsTo != null)
                hashCode = hashCode * 59 + ConformsTo.GetHashCode();
            return hashCode;
        }
    }
}