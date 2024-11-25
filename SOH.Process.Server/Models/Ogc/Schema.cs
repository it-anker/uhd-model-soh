using System.Runtime.Serialization;

namespace SOH.Process.Server.Models.Ogc;

public class Schema
{
    [DataMember(Name = "description")]
    public string? Description { get; set; }
}