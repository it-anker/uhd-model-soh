using System.Runtime.Serialization;
using Mars.Interfaces.Model;
using SOH.Process.Server.Models.Parameters;

namespace SOH.Process.Server.Models.Ogc;

[DataContract]
public class Output
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
}

[DataContract]
public class ProcessInput
{
    /// <summary>
    ///     The concrete config to describe the complete simulation config.
    /// </summary>
    [DataMember(Name = "config")]
    public SimulationConfig? Config { get; set; }

    /// <summary>
    ///     The start of the simulation.
    /// </summary>
    [DataMember(Name = "startPoint")]
    public DateTime? StartPoint { get; set; }

    /// <summary>
    ///     The end of the simulation.
    /// </summary>
    [DataMember(Name = "endPoint")]
    public DateTime? EndPoint { get; set; }
}