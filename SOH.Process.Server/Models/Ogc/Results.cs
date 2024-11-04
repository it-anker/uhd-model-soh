using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;

namespace SOH.Process.Server.Models.Ogc;

/// <summary>
/// </summary>
[DataContract]
public class Results : Dictionary<string, InlineOrRefData>, IEquatable<Results>, InlineResponse200
{
    /// <summary>
    ///     Returns true if Results instances are equal
    /// </summary>
    /// <param name="other">Instance of Results to be compared</param>
    /// <returns>Boolean</returns>
    public bool Equals(Results other)
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
        sb.Append("class Results {\n");
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
        return obj.GetType() == GetType() && Equals((Results)obj);
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

    public static bool operator ==(Results left, Results right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(Results left, Results right)
    {
        return !Equals(left, right);
    }

#pragma warning restore 1591

    #endregion Operators
}