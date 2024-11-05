using System.Runtime.Serialization;
using SOH.Process.Server.Models.Common;

namespace SOH.Process.Server.Models.Ogc;

[DataContract]
public sealed class Results : Dictionary<string, object>, IEquatable<Results>, IInlineResponse200
{
    /// <summary>
    ///     Returns true if Results instances are equal.
    /// </summary>
    /// <param name="other">Instance of Results to be compared.</param>
    /// <returns>Boolean</returns>
    public bool Equals(Results? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;

        return false;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as Results);
    }
}