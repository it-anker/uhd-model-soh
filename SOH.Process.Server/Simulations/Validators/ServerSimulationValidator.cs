using FluentValidation;

namespace SOH.Process.Server.Simulations.Validators;

public class ServerSimulationValidator : AbstractValidator<SimulationProcessDescription>
{
    public ServerSimulationValidator(ISimulationService simulationService)
    {
        RuleFor(description => description.Id)
            .NotEmpty()
            .WithMessage("id required")
            .MaximumLength(GlobalConstants.DefaultMaximumIdLength)
            .WithMessage($"length can only ba a maximum of {GlobalConstants.DefaultMaximumIdLength}");

        RuleFor(description => description.Description)
            .MaximumLength(GlobalConstants.DefaultMaximumLengthDescription)
            .WithMessage($"length can only ba a maximum of {GlobalConstants.DefaultMaximumLength}")
            .Unless(description => string.IsNullOrEmpty(description.Description));

        RuleFor(description => description.Version)
            .NotEmpty()
            .WithMessage("version required")
            .MaximumLength(GlobalConstants.DefaultMaximumLength)
            .WithMessage($"length can only ba a maximum of {GlobalConstants.DefaultMaximumLength}");

        RuleFor(description => description.Title)
            .NotEmpty()
            .WithMessage("title required")
            .MaximumLength(GlobalConstants.DefaultMaximumLength)
            .WithMessage($"length can only ba a maximum of {GlobalConstants.DefaultMaximumLength}");
    }
}