using System.Runtime.Serialization;
using SOH.Process.Server.Models.Processes;

namespace SOH.Process.Server.Simulations;

public class SimulationJob : StatusInfo
{
    [DataMember(Name = "jobStatusKey")]
    public string? JobStatusKey { get; set; }
}