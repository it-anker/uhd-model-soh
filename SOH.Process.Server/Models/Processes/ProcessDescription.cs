using System.Runtime.Serialization;
using SOH.Process.Server.Models.Ogc;
using SOH.Process.Server.Models.Parameters;

namespace SOH.Process.Server.Models.Processes;

[DataContract]
public class ProcessDescription : ProcessSummary
{
    /// <summary>
    ///     All inputs of this process.
    /// </summary>
    [DataMember(Name = "inputs")]
    public Dictionary<string, InputDescription> Inputs { get; set; } = [];

    /// <summary>
    ///     All outputs of this process.
    /// </summary>
    [DataMember(Name = "outputs")]
    public Dictionary<string, OutputDescription> Outputs { get; set; } = [];

    public bool ShouldSerializeInputs() => Inputs != null! && Inputs.Count > 0;
    public bool ShouldSerializeOutputs() => Outputs != null! && Outputs.Count > 0;
}