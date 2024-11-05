using System.Runtime.Serialization;
using SOH.Process.Server.Models.Parameters;

namespace SOH.Process.Server.Models.Ogc;

[DataContract]
public sealed class Output : IEquatable<Output>
{
    /// <summary>
    ///     Gets or Sets Format.
    /// </summary>
    [DataMember(Name = "format")]
    public Format? Format { get; set; }

    /// <summary>
    ///     Gets or Sets TransmissionMode.
    /// </summary>
    [DataMember(Name = "transmissionMode")]
    public TransmissionMode? TransmissionMode { get; set; }

    public bool Equals(Output? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Equals(Format, other.Format) && TransmissionMode == other.TransmissionMode;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((Output)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Format, TransmissionMode);
    }
}