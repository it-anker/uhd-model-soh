using System.Runtime.Serialization;

namespace SOH.Process.Server.Models.Parameters;

[DataContract]
public class Format
{
    /// <summary>
    ///     Gets or Sets MediaType.
    /// </summary>
    [DataMember(Name = "mediaType")]
    public string? MediaType { get; set; }

    /// <summary>
    ///     Gets or Sets Encoding.
    /// </summary>
    [DataMember(Name = "encoding")]
    public string? Encoding { get; set; }

    /// <summary>
    ///     Gets or Sets Schema.
    /// </summary>
    [DataMember(Name = "schema")]
    public object? Schema { get; set; }
}