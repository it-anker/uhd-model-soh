using System.Runtime.Serialization;
using SOH.Process.Server.Models.Ogc;

namespace SOH.Process.Server.Models.Parameters;

[DataContract]
public class DescriptionType
{
    /// <summary>
    ///     A title to display.
    /// </summary>
    [DataMember(Name = "title")]
    public string? Title { get; set; }

    /// <summary>
    ///     An optional description of this element to present.
    /// </summary>
    [DataMember(Name = "description")]
    public string? Description { get; set; }

    /// <summary>
    ///     A list of special keywords.
    /// </summary>
    [DataMember(Name = "keywords")]
    public List<string> Keywords { get; set; } = [];

    /// <summary>
    ///     Gets or sets some additional metadata used external.
    /// </summary>
    [DataMember(Name = "metadata")]
    public List<Metadata> Metadata { get; set; } = [];

    /// <summary>
    ///     Gets or sets some additional parameter used external.
    /// </summary>
    [DataMember(Name = "additionalParameters")]
    public AllOfdescriptionTypeAdditionalParameters? AdditionalParameters { get; set; }
}