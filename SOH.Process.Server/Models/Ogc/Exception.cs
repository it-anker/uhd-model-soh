using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;

namespace SOH.Process.Server.Models.Ogc;

/// <summary>
///     JSON schema for exceptions based on RFC 7807
/// </summary>
[DataContract]
public class Exception : Dictionary<string, object>, IEquatable<Exception>
{
    /// <summary>
    ///     Returns true if Exception instances are equal
    /// </summary>
    /// <param name="other">Instance of Exception to be compared</param>
    /// <returns>Boolean</returns>
    public bool Equals(Exception other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;

        return false && base.Equals(other);
    }

    /// <summary>
    ///     Returns the string presentation of the object
    /// </summary>
    /// <returns>String presentation of the object</returns>
    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("class Exception {\n");
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
        return obj.GetType() == GetType() && Equals((Exception)obj);
    }

    /// <summary>
    ///     Gets the hash code
    /// </summary>
    /// <returns>Hash code</returns>
    public override int GetHashCode()
    {
        int hashCode = 41;
        // Suitable nullity checks etc, of course :)
        return hashCode;
    }

    #region Operators

#pragma warning disable 1591

    public static bool operator ==(Exception left, Exception right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(Exception left, Exception right)
    {
        return !Equals(left, right);
    }

#pragma warning restore 1591

    #endregion Operators
}