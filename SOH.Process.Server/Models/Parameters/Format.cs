using System.Runtime.Serialization;

namespace SOH.Process.Server.Models.Parameters;

[DataContract]
public class Format
{
    /// <summary>
    ///     Gets or sets the media type.
    /// </summary>
    [DataMember(Name = "mediaType")]
    public string? MediaType { get; set; }

    /// <summary>
    ///     Gets or sets the encoding to read.
    /// </summary>
    [DataMember(Name = "encoding")]
    public string? Encoding { get; set; }

    /// <summary>
    ///     Gets or sets the schema.
    /// </summary>
    [DataMember(Name = "schema")]
    public object? Schema { get; set; }
}