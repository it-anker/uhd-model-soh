using System.Runtime.Serialization;
using SOH.Process.Server.Models.Ogc;
using SOH.Process.Server.Models.Parameters;

namespace SOH.Process.Server.Models.Processes;

[DataContract]
public sealed class Process : ProcessSummary, IEquatable<Process>
{
    /// <summary>
    ///     Gets or Sets Inputs.
    /// </summary>
    [DataMember(Name = "inputs")]
    public Dictionary<string, InputDescription> Inputs { get; set; } = [];

    /// <summary>
    ///     Gets or Sets Outputs.
    /// </summary>
    [DataMember(Name = "outputs")]
    public Dictionary<string, OutputDescription> Outputs { get; set; } = [];

    public bool Equals(Process? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return base.Equals(other);
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((Process)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), Inputs, Outputs);
    }
}