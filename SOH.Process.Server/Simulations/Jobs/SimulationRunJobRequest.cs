using System.Runtime.Serialization;
using GTFS.Attributes;
using MediatR;
using SOH.Process.Server.Models.Ogc;

namespace SOH.Process.Server.Simulations.Jobs;

public class SimulationRunJobRequest : IRequest<Unit>
{
    [Required, DataMember(Name = "jobId")]
    public string JobId { get; set; } = null!;

    public bool IsTest { get; set; }
}