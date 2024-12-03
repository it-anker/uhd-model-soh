using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using SOH.Process.Server.Models.Parameters;

namespace SOH.Process.Server.Models.Ogc;

[DataContract]
public class OutputDescription
{
    /// <summary>
    ///     The kind how this output is transmitted to the client.
    /// </summary>
    [DataMember(Name = "transmissionMode")]
    public TransmissionMode? TransmissionMode { get; set; }

    /// <summary>
    ///     An optional description of the underlying schema.
    /// </summary>
    [DataMember(Name = "schema")]
    public Schema? Schema { get; set; }

    /// <summary>
    ///     The format of the output returned.
    /// </summary>
    [DataMember(Name = "format")]
    public Format? Format { get; set; }
}