using System.Runtime.Serialization;
using GTFS.Attributes;
using Mars.Interfaces.Model;
using MediatR;
using SOH.Process.Server.Models.Ogc;

namespace SOH.Process.Server.Simulations.Jobs;

public class SimulationRunJobRequest : IRequest<Unit>
{
    [Required, DataMember(Name = "jobId")]
    public string JobId { get; set; } = default!;

    [DataMember(Name = "execute")]
    public Execute? Execute { get; set; }
}