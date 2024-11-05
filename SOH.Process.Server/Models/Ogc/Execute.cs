using System.Runtime.Serialization;

namespace SOH.Process.Server.Models.Ogc;

[DataContract]
public sealed class Execute : IEquatable<Execute>
{
    /// <summary>
    ///     Gets or Sets Inputs.
    /// </summary>
    [DataMember(Name = "inputs")]
    public Dictionary<string, object> Inputs { get; set; } = [];

    /// <summary>
    ///     Gets or Sets Outputs.
    /// </summary>
    [DataMember(Name = "outputs")]
    public Dictionary<string, Output> Outputs { get; set; } = [];

    /// <summary>
    ///     Gets or Sets Response.
    /// </summary>
    [DataMember(Name = "response")]
    public ResponseKind? Response { get; set; }

    /// <summary>
    ///     Gets or Sets Subscriber.
    /// </summary>
    [DataMember(Name = "subscriber")]
    public Subscriber? Subscriber { get; set; }

    public bool Equals(Execute? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Inputs.Equals(other.Inputs) && Outputs.Equals(other.Outputs) &&
               Response == other.Response && Equals(Subscriber, other.Subscriber);
    }

    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj) || (obj is Execute other && Equals(other));
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Inputs, Outputs, Response, Subscriber);
    }
}