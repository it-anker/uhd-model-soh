using System.Runtime.Serialization;

namespace SOH.Process.Server.Models.Ogc;

[DataContract]
public class Metadata
{
    /// <summary>
    ///     Gets or Sets Title.
    /// </summary>
    [DataMember(Name = "title")]
    public string? Title { get; set; }

    /// <summary>
    ///     Gets or Sets Role.
    /// </summary>
    [DataMember(Name = "role")]
    public string? Role { get; set; }

    /// <summary>
    ///     Gets or Sets Href.
    /// </summary>
    [DataMember(Name = "href")]
    public string? Href { get; set; }
}