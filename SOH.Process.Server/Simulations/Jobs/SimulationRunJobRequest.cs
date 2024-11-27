using MediatR;

namespace SOH.Process.Server.Simulations.Jobs;

public class SimulationRunJobRequest : IRequest<Unit>
{
    public string JobId { get; set; } = default!;
}