using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;

namespace SOH.Process.Server.Models.Ogc;

/// <summary>
/// </summary>
[DataContract]
public class QualifiedInputValue : Format, IEquatable<QualifiedInputValue>, InlineOrRefData
{
    /// <summary>
    ///     Gets or Sets Value
    /// </summary>
    [Required]
    [DataMember(Name = "value")]
    public InputValue Value { get; set; }

    /// <summary>
    ///     Returns true if QualifiedInputValue instances are equal
    /// </summary>
    /// <param name="other">Instance of QualifiedInputValue to be compared</param>
    /// <returns>Boolean</returns>
    public bool Equals(QualifiedInputValue other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;

        return
        (
            Value == other.Value ||
            (Value != null &&
             Value.Equals(other.Value))
        ) && base.Equals(other);
    }

    /// <summary>
    ///     Returns the string presentation of the object
    /// </summary>
    /// <returns>String presentation of the object</returns>
    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("class QualifiedInputValue {\n");
        sb.Append("  Value: ").Append(Value).Append("\n");
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
        return obj.GetType() == GetType() && Equals((QualifiedInputValue)obj);
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
            if (Value != null)
                hashCode = hashCode * 59 + Value.GetHashCode();
            return hashCode;
        }
    }

    #region Operators

#pragma warning disable 1591

    public static bool operator ==(QualifiedInputValue left, QualifiedInputValue right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(QualifiedInputValue left, QualifiedInputValue right)
    {
        return !Equals(left, right);
    }

#pragma warning restore 1591

    #endregion Operators
}