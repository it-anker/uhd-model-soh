using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using SOH.Process.Server.Models.Parameters;

namespace SOH.Process.Server.Models.Ogc;

[DataContract]
public class OutputDescription : DescriptionType
{
    [Required]
    [DataMember(Name = "schema")]
    public Schema Schema { get; set; } = default!;
}