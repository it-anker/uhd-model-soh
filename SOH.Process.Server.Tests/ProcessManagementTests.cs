using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SOH.Process.Server.Simulations;
using SOH.Process.Server.Tests.Base;

namespace SOH.Process.Server.Tests;

public class ProcessManagementTests : AbstractManagementTests
{
    private IRequestHandler<CreateSimulationProcessDescriptionRequest, SimulationProcessDescription> _createProcessHandler;

    public ProcessManagementTests(OgcIntegration services) : base(services)
    {
        _createProcessHandler = Services.GetRequiredService<
            IRequestHandler<CreateSimulationProcessDescriptionRequest, SimulationProcessDescription>>();
    }
}