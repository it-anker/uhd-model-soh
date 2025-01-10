using FluentValidation;

namespace SOH.Process.Server.Simulations.Validators;

public class ServerCreateSimulationJobRequestValidator : AbstractValidator<CreateSimulationJobRequest>
{
    private readonly ISimulationService _simulationService;
    private SimulationProcessDescription? _process;
    private List<string> _missingRequiredInputs = [];

    public ServerCreateSimulationJobRequestValidator(ISimulationService simulationService)
    {
        _simulationService = simulationService;

        WhenAsync(async (request, token) => await IsValidProcess(request, token), () =>
        {
            RuleFor(request => request.Execute)
                .ChildRules(rules =>
                {
                    rules.RuleForEach(execute => execute.Inputs)
                        .Must(pair => _process!.Inputs.ContainsKey(pair.Key))
                        .WithMessage(
                            (_, pair) => $"Selected input '{pair.Key}' is not provided by process {_process!.Id}");

                    rules.RuleForEach(execute => execute.Outputs)
                        .Must(pair => _process!.Outputs.ContainsKey(pair.Key))
                        .WithMessage((_, pair) =>
                            $"Selected output '{pair.Key}' is not provided by process {_process!.Id}");

                    rules.RuleFor(execute => execute.Inputs)
                        .Must(allInputs =>
                        {
                            var required = _process!.Inputs
                                .Where(pair => pair.Value.MinOccurs > 1);

                            foreach (string inputName in required.Select(pair => pair.Key))
                            {
                                if (!allInputs.ContainsKey(inputName))
                                {
                                    _missingRequiredInputs.Add(inputName);
                                }
                            }
                            return _missingRequiredInputs.Count <= 0;
                        })
                        .WithMessage($"[{string.Join(",", $"'{_missingRequiredInputs}'")}] inputs required");
                });

        });
    }

    private async Task<bool> IsValidProcess(CreateSimulationJobRequest request, CancellationToken token)
    {
        _process = await _simulationService.GetSimulationAsync(request.SimulationId, token);
        return _process != null;
    }
}