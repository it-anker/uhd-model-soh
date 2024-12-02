using Mapster;
using Microsoft.Extensions.Localization;
using SOH.Process.Server.Models.Ogc;
using SOH.Process.Server.Resources;

namespace SOH.Process.Server.Simulations;

internal class ModelSeeder(
    ISimulationService simulationService,
    IConfiguration configuration,
    IStringLocalizer<SharedResource> localizer) : ICustomSeeder
{
    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        await SeedFerrySimulationProcess(cancellationToken);
    }

    private async Task SeedFerrySimulationProcess(CancellationToken cancellationToken)
    {
        string host = configuration["ApiBaseUrl"] !;
        if (!host.EndsWith('/')) host += "/";

        var ferryModel = new CreateSimulationProcessRequest
        {
            ExecutionKind = ProcessExecutionKind.Direct,
            Title = localizer["ferry_transfer_model_title"],
            Description = localizer["ferry_transfer_model_description"],
            Version = "1.0.0",
            Keywords = ["ferry", "transfer", "simulation"],
            JobControlOptions =
            [
                JobControlOptions.SynchronousExecution,
                JobControlOptions.AsyncExecution
            ],
            OutputTransmission =
            [
                TransmissionMode.ValueEnum
            ],
            Outputs = new Dictionary<string, OutputDescription>
            {
                {
                    "agentsOutput", new OutputDescription
                    {
                        Description = localizer["ferry_transfer_output_agents"],
                        Keywords = ["point", "result", "agent"],
                        Title = localizer["ferry_transfer_output_agents_title"]
                    }
                }
            },
            Links = [
                new Link
                {
                    Href = new Uri(host + $"/processes/{GlobalConstants.FerryTransferId}").ToString(),
                    Title = localizer["model_self_description"],
                    Rel = "self",
                    Type = "application/json"
                }
            ]
        };

        var existingProcess = await simulationService.FindSimulationAsync(
            GlobalConstants.FerryTransferId, cancellationToken);

        if (existingProcess == null)
        {
            await simulationService.CreateAsync(GlobalConstants.FerryTransferId,
                ferryModel, cancellationToken);
        }
        else
        {
            var update = ferryModel.Adapt<UpdateSimulationProcessRequest>();
            await simulationService.UpdateAsync(GlobalConstants.FerryTransferId,
                update, cancellationToken);
        }
    }
}