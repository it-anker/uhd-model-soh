using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;

namespace SOH.Process.Server.Models.Ogc;

/// <summary>
/// </summary>
[DataContract]
public class OutputDescription : DescriptionType, IEquatable<OutputDescription>
{
    /// <summary>
    ///     Gets or Sets Schema
    /// </summary>
    [Required]
    [DataMember(Name = "schema")]
    public Schema Schema { get; set; }

    /// <summary>
    ///     Returns true if OutputDescription instances are equal
    /// </summary>
    /// <param name="other">Instance of OutputDescription to be compared</param>
    /// <returns>Boolean</returns>
    public bool Equals(OutputDescription other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;

        return
        (
            Schema == other.Schema ||
            (Schema != null &&
             Schema.Equals(other.Schema))
        ) && base.Equals(other);
    }

    /// <summary>
    ///     Returns the string presentation of the object
    /// </summary>
    /// <returns>String presentation of the object</returns>
    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("class OutputDescription {\n");
        sb.Append("  Schema: ").Append(Schema).Append("\n");
        sb.Append("}\n");
        return sb.ToString();
    }

    /// <summary>
    ///     Returns the JSON string presentation of the object
    /// </summary>
    /// <returns>JSON string presentation of the object</returns>
    public new string ToJson()
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
        return obj.GetType() == GetType() && Equals((OutputDescription)obj);
    }

    /// <summary>
    ///     Gets the hash code
    /// </summary>
    /// <returns>Hash code</returns>
    public override int GetHashCode()
    {
        unchecked // Overflow is fine, just wrap
        {
            int hashCode = 41;
            // Suitable nullity checks etc, of course :)
            if (Schema != null)
                hashCode = hashCode * 59 + Schema.GetHashCode();
            return hashCode;
        }
    }

    #region Operators

#pragma warning disable 1591

    public static bool operator ==(OutputDescription left, OutputDescription right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(OutputDescription left, OutputDescription right)
    {
        return !Equals(left, right);
    }

#pragma warning restore 1591

    #endregion Operators
}