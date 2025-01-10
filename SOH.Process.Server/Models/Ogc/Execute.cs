using System.Runtime.Serialization;

namespace SOH.Process.Server.Models.Ogc;

/// <summary>
///     The common contract to request an execution of a simulation
///     with <see cref="Inputs"/> and <see cref="Outputs"/> as the result.
/// </summary>
[DataContract]
public class Execute
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

    /// <summary>
    ///     Gets or an optional identifier.
    /// </summary>
    [DataMember(Name = "jobIdentifier")]
    public string? JobIdentifier { get; set; }

    public bool ShouldSerializeInputs() => Inputs != null! && Inputs.Count > 0;
    public bool ShouldSerializeOutputs() => Outputs != null! && Outputs.Count > 0;
}