using SOH.Process.Server.Models.Ogc;
using SOH.Process.Server.Models.Processes;

namespace SOH.Process.Server.Simulations;

internal class ModelSeeder(ISimulationService simulationService) : ICustomSeeder
{
    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        var ferryModel = new CreateSimulationProcessRequest
        {
            Title = "SOH - Ferry Transfer Model",
            Description = "Simple transfer model to of the Hamburg HADAG ferry system.",
            Version = "1.0.0",
            Keywords = ["ferry", "transfer", "simulation"],
            JobControlOptions = [
                JobControlOptions.SynchronousExecution,
                JobControlOptions.AsyncExecution
            ],
            OutputTransmission = [
                TransmissionMode.ValueEnum
            ]
        };

        var existingProcess = await simulationService.ListProcessesAsync(
            new ParameterLimit(), cancellationToken);
        if (existingProcess.Processes.TrueForAll(summary => summary.Title != ferryModel.Title))
        {
            await simulationService.CreateAsync(ferryModel, cancellationToken);
        }
    }
}