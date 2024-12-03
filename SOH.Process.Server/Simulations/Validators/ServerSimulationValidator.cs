using FluentValidation;

namespace SOH.Process.Server.Simulations.Validators;

public class ServerSimulationValidator : AbstractValidator<SimulationProcessDescription>
{
    public ServerSimulationValidator()
    {
    }
}