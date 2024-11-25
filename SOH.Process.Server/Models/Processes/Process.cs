using System.Runtime.Serialization;
using SOH.Process.Server.Models.Ogc;
using SOH.Process.Server.Models.Parameters;

namespace SOH.Process.Server.Models.Processes;

[DataContract]
public class Process : ProcessSummary
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
}