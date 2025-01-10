using System.Runtime.Serialization;
using SOH.Process.Server.Models.Ogc;

namespace SOH.Process.Server.Models.Parameters;

[DataContract]
public sealed class AllOfdescriptionTypeAdditionalParameters : Metadata,
    IEquatable<AllOfdescriptionTypeAdditionalParameters>
{
    /// <summary>
    ///     Gets or Sets Parameters.
    /// </summary>
    [DataMember(Name = "parameters")]
    public List<AdditionalParameter> Parameters { get; set; } = [];

    public bool ShouldSerializeParameters() => Parameters != null! && Parameters.Count > 0;

    public bool Equals(AllOfdescriptionTypeAdditionalParameters? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return base.Equals(other) && Parameters.Equals(other.Parameters);
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((AllOfdescriptionTypeAdditionalParameters)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), Parameters);
    }
}