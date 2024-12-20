using System.Diagnostics;
using Mars.Common.Core.Logging;
using Mars.Components.Starter;
using Mars.Core.Simulation;
using Mars.Interfaces;
using Mars.Interfaces.Model;
using SOHModel;

namespace SOHFerryTransferBox;

internal static class Program
{
    /// <summary>
    ///     The entry point when executing the ferry transfer model without any orchestrating service.
    /// </summary>
    private static void Main(string[] args)
    {
        var description = Startup.CreateModelDescription();

        ISimulationContainer application;
        if (args.Length != 0)
        {
            application = SimulationStarter.BuildApplication(description, args);
        }
        else
        {
            string file = File.ReadAllText("ferry_transfer_config.json");
            var simConfig = SimulationConfig.Deserialize(file);
            application = SimulationStarter.BuildApplication(description, simConfig);
        }

        var simulation = application.Resolve<ISimulation>();

        var watch = Stopwatch.StartNew();
        var state = simulation.StartSimulation();
        watch.Stop();

        Console.WriteLine($"Executed iterations {state.Iterations} lasted {watch.Elapsed}");
    }
}