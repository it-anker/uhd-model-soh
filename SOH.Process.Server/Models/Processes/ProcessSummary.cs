using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using SOH.Process.Server.Models.Ogc;
using SOH.Process.Server.Models.Parameters;

namespace SOH.Process.Server.Models.Processes;

[DataContract]
public class ProcessSummary : AbstractEntity
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

    /// <summary>
    ///     Gets or Sets Version.
    /// </summary>
    [Required]
    [DataMember(Name = "version")]
    public string Version { get; set; } = default!;

    /// <summary>
    ///     Gets or Sets JobControlOptions.
    /// </summary>
    [DataMember(Name = "jobControlOptions")]
    public List<JobControlOptions> JobControlOptions { get; set; } = [];

    /// <summary>
    ///     Gets or Sets OutputTransmission.
    /// </summary>
    [DataMember(Name = "outputTransmission")]
    public List<TransmissionMode> OutputTransmission { get; set; } = [];

    /// <summary>
    ///     Gets or Sets Links.
    /// </summary>
    [DataMember(Name = "links")]
    public List<Link> Links { get; set; } = [];
}