using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;

namespace SOH.Process.Server.Models.Ogc;

/// <summary>
///     Optional URIs for callbacks for this job.  Support for this parameter is not required and the parameter may be
///     removed from the API definition, if conformance class **&#x27;callback&#x27;** is not listed in the conformance
///     declaration under &#x60;/conformance&#x60;.
/// </summary>
[DataContract]
public class Subscriber : IEquatable<Subscriber>
{
    /// <summary>
    ///     Gets or Sets SuccessUri
    /// </summary>

    [DataMember(Name = "successUri")]
    public string SuccessUri { get; set; }

    /// <summary>
    ///     Gets or Sets InProgressUri
    /// </summary>

    [DataMember(Name = "inProgressUri")]
    public string InProgressUri { get; set; }

    /// <summary>
    ///     Gets or Sets FailedUri
    /// </summary>

    [DataMember(Name = "failedUri")]
    public string FailedUri { get; set; }

    /// <summary>
    ///     Returns true if Subscriber instances are equal
    /// </summary>
    /// <param name="other">Instance of Subscriber to be compared</param>
    /// <returns>Boolean</returns>
    public bool Equals(Subscriber other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;

        return
            (
                SuccessUri == other.SuccessUri ||
                (SuccessUri != null &&
                 SuccessUri.Equals(other.SuccessUri))
            ) &&
            (
                InProgressUri == other.InProgressUri ||
                (InProgressUri != null &&
                 InProgressUri.Equals(other.InProgressUri))
            ) &&
            (
                FailedUri == other.FailedUri ||
                (FailedUri != null &&
                 FailedUri.Equals(other.FailedUri))
            );
    }

    /// <summary>
    ///     Returns the string presentation of the object
    /// </summary>
    /// <returns>String presentation of the object</returns>
    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("class Subscriber {\n");
        sb.Append("  SuccessUri: ").Append(SuccessUri).Append("\n");
        sb.Append("  InProgressUri: ").Append(InProgressUri).Append("\n");
        sb.Append("  FailedUri: ").Append(FailedUri).Append("\n");
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
        return obj.GetType() == GetType() && Equals((Subscriber)obj);
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
            if (SuccessUri != null)
                hashCode = hashCode * 59 + SuccessUri.GetHashCode();
            if (InProgressUri != null)
                hashCode = hashCode * 59 + InProgressUri.GetHashCode();
            if (FailedUri != null)
                hashCode = hashCode * 59 + FailedUri.GetHashCode();
            return hashCode;
        }
    }

    #region Operators

#pragma warning disable 1591

    public static bool operator ==(Subscriber left, Subscriber right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(Subscriber left, Subscriber right)
    {
        return !Equals(left, right);
    }

#pragma warning restore 1591

    #endregion Operators
}