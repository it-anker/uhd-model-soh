using Mapster;
using Microsoft.Extensions.Localization;
using NetTopologySuite.Features;
using SOH.Process.Server.Models.Ogc;
using SOH.Process.Server.Models.Parameters;
using SOH.Process.Server.Resources;

namespace SOH.Process.Server.Simulations;

internal class ModelSeeder(
    ISimulationService simulationService,
    IConfiguration configuration,
    IStringLocalizer<SharedResource> localization) : ICustomSeeder
{
    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        await SeedFerrySimulationProcess(cancellationToken);
        await SeedGreen4BikesSimulationProcess(cancellationToken);
#if DEBUG

        var testModel = new CreateSimulationProcessDescriptionRequest
        {
            ExecutionKind = ProcessExecutionKind.Direct,
            Title = "TestModel",
            Description = "TestDesc",
            Version = "0.0.1",
            Keywords = ["test"],
            IsTest = true,
            JobControlOptions =
            [
                JobControlOptions.SynchronousExecution
            ],
            OutputTransmission =
            [
                TransmissionMode.Value
            ],
            Outputs = new Dictionary<string, OutputDescription>
            {
                {
                    "default", new OutputDescription
                    {
                        Format = new Format
                        {
                            MediaType = "application/geo+json"
                        },
                        Schema = new Schema
                        {
                            Title = "Default test output",
                            ContentMediaType = "application/geo+json",
                            Default = new FeatureCollection()
                        }
                    }
                }
            }
        };

        const string testId = "sim-testProcessId";
        var testProcess = await simulationService.FindSimulationAsync(testId, cancellationToken);
        if (testProcess == null)
        {
            await simulationService.CreateAsync(testId, testModel, cancellationToken);
        }
#endif
    }

    private async Task SeedFerrySimulationProcess(CancellationToken cancellationToken)
    {
        string host = configuration["ApiBaseUrl"] !;
        if (!host.EndsWith('/')) host += "/";

        var ferryModel = new CreateSimulationProcessDescriptionRequest
        {
            ExecutionKind = ProcessExecutionKind.Direct,
            Title = localization["ferry_transfer_model_title"],
            Description = localization["ferry_transfer_model_description"],
            Version = "1.0.0",
            Keywords = ["ferry", "transfer", "simulation"],
            JobControlOptions =
            [
                JobControlOptions.SynchronousExecution,
                JobControlOptions.AsyncExecution
            ],
            OutputTransmission =
            [
                TransmissionMode.Value
            ],
            Inputs = ModelSeederOutputs.GetSmartOpenHamburgInputs(localization),
            Outputs = ModelSeederOutputs.GetSmartOpenHamburgOutputs(localization),
            Links =
            [
                new Link
                {
                    Href = new Uri(host + $"/processes/{GlobalConstants.FerryTransferId}").ToString(),
                    Title = localization["model_self_description"],
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
            var update = ferryModel.Adapt<UpdateSimulationProcessDescriptionRequest>();
            await simulationService.UpdateAsync(GlobalConstants.FerryTransferId,
                update, cancellationToken);
        }
    }

    private async Task SeedGreen4BikesSimulationProcess(CancellationToken cancellationToken)
    {
        string host = configuration["ApiBaseUrl"] !;
        if (!host.EndsWith('/')) host += "/";

        var ferryModel = new CreateSimulationProcessDescriptionRequest
        {
            ExecutionKind = ProcessExecutionKind.Direct,
            Title = localization["green_4_bikes_model_title"],
            Description = localization["green_4_bikes_model_description"],
            Version = "1.0.0",
            Keywords = ["cycling", "travelling", "simulation"],
            JobControlOptions =
            [
                JobControlOptions.SynchronousExecution,
                JobControlOptions.AsyncExecution
            ],
            OutputTransmission =
            [
                TransmissionMode.Value
            ],
            Inputs = ModelSeederOutputs.GetSmartOpenHamburgInputs(localization),
            Outputs = ModelSeederOutputs.GetSmartOpenHamburgOutputs(localization),
            Links =
            [
                new Link
                {
                    Href = new Uri(host + $"/processes/{GlobalConstants.FerryTransferId}").ToString(),
                    Title = localization["model_self_description"],
                    Rel = "self",
                    Type = "application/json"
                }
            ]
        };

        var existingProcess = await simulationService.FindSimulationAsync(
            GlobalConstants.Green4BikesId, cancellationToken);

        if (existingProcess == null)
        {
            await simulationService.CreateAsync(GlobalConstants.Green4BikesId,
                ferryModel, cancellationToken);
        }
        else
        {
            var update = ferryModel.Adapt<UpdateSimulationProcessDescriptionRequest>();
            await simulationService.UpdateAsync(GlobalConstants.Green4BikesId,
                update, cancellationToken);
        }
    }
}