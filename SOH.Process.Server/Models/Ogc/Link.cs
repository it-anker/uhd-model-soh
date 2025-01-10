using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace SOH.Process.Server.Models.Ogc;

[DataContract]
public sealed class Link
{
    /// <summary>
    ///     Gets or Sets Href.
    /// </summary>
    [Required]
    [DataMember(Name = "href")]
    public string Href { get; set; } = null!;

    /// <summary>
    ///     Gets or Sets Rel.
    /// </summary>
    [DataMember(Name = "rel")]
    public string? Rel { get; set; }

    /// <summary>
    ///     Gets or Sets Type.
    /// </summary>
    [DataMember(Name = "type")]
    public string? Type { get; set; }

    /// <summary>
    ///     Gets or Sets Hreflang.
    /// </summary>
    [DataMember(Name = "hreflang")]
    public string? Hreflang { get; set; }

    /// <summary>
    ///     Gets or Sets Title.
    /// </summary>
    [DataMember(Name = "title")]
    public string? Title { get; set; }

    public Link Trim()
    {
        Href = Href.Trim();
        Title = !string.IsNullOrEmpty(Title) ? Title.Trim() : null;
        Rel = !string.IsNullOrEmpty(Rel) ? Rel.Trim() : null;
        Hreflang = !string.IsNullOrEmpty(Hreflang) ? Hreflang.Trim() : null;
        Type = !string.IsNullOrEmpty(Type) ? Type.Trim() : null;
        return this;
    }
}