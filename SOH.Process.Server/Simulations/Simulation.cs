using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using SOH.Process.Server.Models.Processes;

namespace SOH.Process.Server.Simulations;

public class Simulation
{
    [Required]
    [DataMember(Name = "id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [DataMember(Name = "jobId")]
    public string? JobId { get; set; }

    [DataMember(Name = "resultId")]
    public string? ResultId { get; set; }

    [DataMember(Name = "status")]
    public StatusInfo? Status { get; set; }
}

public class UpdateSimulationRequest : Simulation
{
} 

public class CreateSimulationRequest
{
}