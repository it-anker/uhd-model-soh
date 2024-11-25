using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace SOH.Process.Server.Models.Ogc;

[DataContract]
public sealed class ConfClasses
{
    /// <summary>
    ///     Gets or Sets ConformsTo.
    /// </summary>
    [Required]
    [DataMember(Name = "conformsTo")]
    public List<string> ConformsTo { get; set; } = [];
}