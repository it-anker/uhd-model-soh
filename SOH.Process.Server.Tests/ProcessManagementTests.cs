using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SOH.Process.Server.Simulations;

namespace SOH.Process.Server.Tests;

public class ProcessManagementTests : AbstractManagementTests
{
    private IRequestHandler<CreateSimulationProcessRequest, SimulationProcess> _createProcessHandler;

    public ProcessManagementTests(OgcIntegration services) : base(services)
    {
        _createProcessHandler = Services.GetRequiredService<
            IRequestHandler<CreateSimulationProcessRequest, SimulationProcess>>();
    }
}