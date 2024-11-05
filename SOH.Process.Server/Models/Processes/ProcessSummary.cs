using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using SOH.Process.Server.Models.Ogc;
using SOH.Process.Server.Models.Parameters;

namespace SOH.Process.Server.Models.Processes;

[DataContract]
public class ProcessSummary : DescriptionType
{
    /// <summary>
    ///     Gets or Sets Id.
    /// </summary>
    [Required]
    [DataMember(Name = "id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();

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

    public bool Equals(ProcessSummary? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return base.Equals(other) && Id == other.Id;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((ProcessSummary)obj);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}