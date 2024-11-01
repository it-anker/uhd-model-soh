using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using Mars.Common.Core.Logging;
using Mars.Components.Starter;
using Mars.Core.Simulation;
using Mars.Core.Simulation.Entities;
using Mars.Interfaces;
using Mars.Interfaces.Model;
using SOHModel.Bicycle.Rental;
using SOHModel.Domain.Graph;
using SOHModel.Multimodal.Model;
using SOHModel.Multimodal.Routing;

namespace SOHGreen4BikesBox;

/// <summary>
///     This pre-defined starter program runs the the Green4Bike scenario with outside passed arguments or
///     a default simulation inputConfiguration with CSV output and trips.
/// </summary>
internal static class Program
{
    public static void Main(string[] args)
    {
        Thread.CurrentThread.CurrentCulture = new CultureInfo("EN-US");
        LoggerFactory.SetLogLevel(LogLevel.Off);

        ModelDescription description = new ModelDescription();

        description.AddLayer<SpatialGraphMediatorLayer>(new[] { typeof(ISpatialGraphLayer) });
        description.AddLayer<GatewayLayer>();
        description.AddLayer<BicycleRentalLayer>();
        description.AddLayer<CycleTravelerLayer>();
        description.AddLayer<CycleTravelerSchedulerLayer>();

        description.AddAgent<CycleTraveler, CycleTravelerLayer>();
        description.AddEntity<RentalBicycle>();

        ISimulationContainer application;

        if (args != null && args.Length != 0)
        {
            application = SimulationStarter.BuildApplication(description, args);
        }
        else
        {
            string file = File.ReadAllText("config.json");
            SimulationConfig? simConfig = SimulationConfig.Deserialize(file);
            application = SimulationStarter.BuildApplication(description, simConfig);
        }

        ISimulation? simulation = application.Resolve<ISimulation>();

        Stopwatch watch = Stopwatch.StartNew();
        SimulationWorkflowState? state = simulation.StartSimulation();
        watch.Stop();

        Console.WriteLine($"Executed iterations {state.Iterations} lasted {watch.Elapsed}");
        application.Dispose();
    }
}