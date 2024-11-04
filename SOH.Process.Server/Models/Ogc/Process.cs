using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;

namespace SOH.Process.Server.Models.Ogc;

/// <summary>
/// </summary>
[DataContract]
public class Process : ProcessSummary, IEquatable<Process>
{
    /// <summary>
    ///     Gets or Sets Inputs
    /// </summary>

    [DataMember(Name = "inputs")]
    public Dictionary<string, InputDescription> Inputs { get; set; }

    /// <summary>
    ///     Gets or Sets Outputs
    /// </summary>

    [DataMember(Name = "outputs")]
    public Dictionary<string, OutputDescription> Outputs { get; set; }

    /// <summary>
    ///     Returns true if Process instances are equal
    /// </summary>
    /// <param name="other">Instance of Process to be compared</param>
    /// <returns>Boolean</returns>
    public bool Equals(Process other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;

        return
            (
                Inputs == other.Inputs ||
                (Inputs != null &&
                 Inputs.SequenceEqual(other.Inputs))
            ) &&
            (
                Outputs == other.Outputs ||
                (Outputs != null &&
                 Outputs.SequenceEqual(other.Outputs))
            ) && base.Equals(other);
    }

    /// <summary>
    ///     Returns the string presentation of the object
    /// </summary>
    /// <returns>String presentation of the object</returns>
    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("class Process {\n");
        sb.Append("  Inputs: ").Append(Inputs).Append("\n");
        sb.Append("  Outputs: ").Append(Outputs).Append("\n");
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
        return obj.GetType() == GetType() && Equals((Process)obj);
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
            if (Inputs != null)
                hashCode = hashCode * 59 + Inputs.GetHashCode();
            if (Outputs != null)
                hashCode = hashCode * 59 + Outputs.GetHashCode();
            return hashCode;
        }
    }

    #region Operators

#pragma warning disable 1591

    public static bool operator ==(Process left, Process right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(Process left, Process right)
    {
        return !Equals(left, right);
    }

#pragma warning restore 1591

    #endregion Operators
}