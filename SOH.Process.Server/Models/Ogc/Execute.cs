using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SOH.Process.Server.Models.Ogc;

/// <summary>
/// </summary>
[DataContract]
public class Execute : IEquatable<Execute>
{
    /// <summary>
    ///     Gets or Sets Response
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ResponseEnum
    {
        /// <summary>
        ///     Enum RawEnum for raw
        /// </summary>
        [EnumMember(Value = "raw")] RawEnum = 0,

        /// <summary>
        ///     Enum DocumentEnum for document
        /// </summary>
        [EnumMember(Value = "document")] DocumentEnum = 1
    }

    /// <summary>
    ///     Gets or Sets Inputs
    /// </summary>

    [DataMember(Name = "inputs")]
    public Dictionary<string, object> Inputs { get; set; }

    /// <summary>
    ///     Gets or Sets Outputs
    /// </summary>

    [DataMember(Name = "outputs")]
    public Dictionary<string, Output> Outputs { get; set; }

    /// <summary>
    ///     Gets or Sets Response
    /// </summary>

    [DataMember(Name = "response")]
    public ResponseEnum? Response { get; set; }

    /// <summary>
    ///     Gets or Sets Subscriber
    /// </summary>

    [DataMember(Name = "subscriber")]
    public Subscriber Subscriber { get; set; }

    /// <summary>
    ///     Returns true if Execute instances are equal
    /// </summary>
    /// <param name="other">Instance of Execute to be compared</param>
    /// <returns>Boolean</returns>
    public bool Equals(Execute other)
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
            ) &&
            (
                Response == other.Response ||
                (Response != null &&
                 Response.Equals(other.Response))
            ) &&
            (
                Subscriber == other.Subscriber ||
                (Subscriber != null &&
                 Subscriber.Equals(other.Subscriber))
            );
    }

    /// <summary>
    ///     Returns the string presentation of the object
    /// </summary>
    /// <returns>String presentation of the object</returns>
    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("class Execute {\n");
        sb.Append("  Inputs: ").Append(Inputs).Append("\n");
        sb.Append("  Outputs: ").Append(Outputs).Append("\n");
        sb.Append("  Response: ").Append(Response).Append("\n");
        sb.Append("  Subscriber: ").Append(Subscriber).Append("\n");
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
        return obj.GetType() == GetType() && Equals((Execute)obj);
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
            if (Response != null)
                hashCode = hashCode * 59 + Response.GetHashCode();
            if (Subscriber != null)
                hashCode = hashCode * 59 + Subscriber.GetHashCode();
            return hashCode;
        }
    }

    #region Operators

#pragma warning disable 1591

    public static bool operator ==(Execute left, Execute right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(Execute left, Execute right)
    {
        return !Equals(left, right);
    }

#pragma warning restore 1591

    #endregion Operators
}