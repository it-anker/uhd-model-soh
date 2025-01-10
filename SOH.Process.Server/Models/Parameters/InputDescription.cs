using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using SOH.Process.Server.Models.Ogc;

namespace SOH.Process.Server.Models.Parameters;

[DataContract]
public sealed class InputDescription : DescriptionType
{
    /// <summary>
    ///     Gets or sets the minimum required elements.
    /// </summary>
    [DataMember(Name = "minOccurs")]
    [DefaultValue(1)]
    public int MinOccurs { get; set; } = 1;

    /// <summary>
    ///     Gets or sets the maximum required elements.
    /// </summary>
    [DataMember(Name = "maxOccurs")]
    public object? MaxOccurs { get; set; }

    /// <summary>
    ///     Gets or sets the underlying schema used to describe this input.
    /// </summary>
    [Required]
    [DataMember(Name = "schema")]
    public Schema Schema { get; set; } = null!;
}