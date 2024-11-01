using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Mars.Common.Core.Logging;
using Mars.Components.Starter;
using Mars.Core.Simulation;
using Mars.Core.Simulation.Entities;
using Mars.Interfaces;
using Mars.Interfaces.Model;
using SOHModel.Domain.Graph;
using SOHModel.Ferry.Model;
using SOHModel.Ferry.Route;
using SOHModel.Ferry.Station;
using SOHModel.Multimodal.Model;

namespace SOHFerryTransferBox;

internal static class Program
{
    private static void Main(string[] args)
    {
        // Thread.CurrentThread.CurrentCulture = new CultureInfo("EN-US");
        LoggerFactory.SetLogLevel(LogLevel.Info);

        ModelDescription description = new ModelDescription();
        description.AddLayer<FerryLayer>();
        description.AddLayer<FerrySchedulerLayer>();
        description.AddLayer<FerryStationLayer>(new[] { typeof(IFerryStationLayer) });
        description.AddLayer<FerryRouteLayer>();
        description.AddLayer<DockWorkerLayer>();
        description.AddLayer<DockWorkerSchedulerLayer>();
        description.AddLayer<SpatialGraphMediatorLayer>(new[] { typeof(ISpatialGraphLayer) });

        description.AddAgent<FerryDriver, FerryLayer>();
        description.AddAgent<DockWorker, DockWorkerLayer>();

        description.AddEntity<Ferry>();

        ISimulationContainer application;
        if (args != null && args.Any())
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
    }
}