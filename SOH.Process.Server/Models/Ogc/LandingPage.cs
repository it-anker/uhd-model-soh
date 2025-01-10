using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace SOH.Process.Server.Models.Ogc;

[DataContract]
public class LandingPage
{
    /// <summary>
    ///     Gets or Sets Title.
    /// </summary>
    [DataMember(Name = "title")]
    public string? Title { get; set; }

    /// <summary>
    ///     Gets or Sets Description.
    /// </summary>
    [DataMember(Name = "description")]
    public string? Description { get; set; }

    /// <summary>
    ///     Gets or Sets Links.
    /// </summary>
    [Required]
    [DataMember(Name = "links")]
    public List<Link> Links { get; set; } = [];

    public bool ShouldSerializeLinks() => Links != null! && Links.Count > 0;
}