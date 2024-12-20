using System.Diagnostics;
using Mars.Components.Starter;
using Mars.Core.Simulation;
using Mars.Interfaces;
using Mars.Interfaces.Model;
using SOHModel;

namespace SOHGreen4BikesBox;

/// <summary>
///     This pre-defined starter program runs the Green4Bike scenario with outside passed arguments or
///     a default simulation inputConfiguration with CSV output and trips.
/// </summary>
public static class Program
{
    /// <summary>
    ///     The entry point when executing the green4bikes model without any orchestrating service.
    /// </summary>
    /// <param name="args">Arguments and config passed from outside.</param>
    public static void Main(string[] args)
    {
        var description = Startup.CreateModelDescription();
        ISimulationContainer application;

        if (args.Length != 0)
        {
            application = SimulationStarter.BuildApplication(description, args);
        }
        else
        {
            string file = File.ReadAllText("green_4_bikes_config.json");
            var simConfig = SimulationConfig.Deserialize(file);
            application = SimulationStarter.BuildApplication(description, simConfig);
        }

        var simulation = application.Resolve<ISimulation>();

        var watch = Stopwatch.StartNew();
        var state = simulation.StartSimulation();
        watch.Stop();

        Console.WriteLine($"Executed iterations {state.Iterations} lasted {watch.Elapsed}");
        application.Dispose();
    }
}