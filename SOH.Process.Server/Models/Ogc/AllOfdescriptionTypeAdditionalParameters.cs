using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;

namespace SOH.Process.Server.Models.Ogc;

/// <summary>
/// </summary>
[DataContract]
public class AllOfdescriptionTypeAdditionalParameters : Metadata, IEquatable<AllOfdescriptionTypeAdditionalParameters>
{
    /// <summary>
    ///     Gets or Sets Parameters
    /// </summary>

    [DataMember(Name = "parameters")]
    public List<AdditionalParameter> Parameters { get; set; }

    /// <summary>
    ///     Returns true if AllOfdescriptionTypeAdditionalParameters instances are equal
    /// </summary>
    /// <param name="other">Instance of AllOfdescriptionTypeAdditionalParameters to be compared</param>
    /// <returns>Boolean</returns>
    public bool Equals(AllOfdescriptionTypeAdditionalParameters other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;

        return
        (
            Parameters == other.Parameters ||
            (Parameters != null &&
             Parameters.SequenceEqual(other.Parameters))
        ) && base.Equals(other);
    }

    /// <summary>
    ///     Returns the string presentation of the object
    /// </summary>
    /// <returns>String presentation of the object</returns>
    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("class AllOfdescriptionTypeAdditionalParameters {\n");
        sb.Append("  Parameters: ").Append(Parameters).Append("\n");
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
        return obj.GetType() == GetType() && Equals((AllOfdescriptionTypeAdditionalParameters)obj);
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
            if (Parameters != null)
                hashCode = hashCode * 59 + Parameters.GetHashCode();
            return hashCode;
        }
    }
}