using System.Runtime.Serialization;
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