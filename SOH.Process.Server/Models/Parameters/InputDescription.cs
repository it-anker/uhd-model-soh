using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using SOH.Process.Server.Models.Ogc;

namespace SOH.Process.Server.Models.Parameters;

[DataContract]
public sealed class InputDescription : DescriptionType
{
    /// <summary>
    ///     Gets or Sets MinOccurs.
    /// </summary>
    [DataMember(Name = "minOccurs")]
    public int? MinOccurs { get; set; }

    /// <summary>
    ///     Gets or Sets MaxOccurs.
    /// </summary>
    [DataMember(Name = "maxOccurs")]
    public object? MaxOccurs { get; set; }

    /// <summary>
    ///     Gets or Sets Schema.
    /// </summary>
    [Required]
    [DataMember(Name = "schema")]
    public Schema Schema { get; set; } = default!;
}