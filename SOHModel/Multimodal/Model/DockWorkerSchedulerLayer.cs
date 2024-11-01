using System.Data;
using Mars.Common;
using Mars.Common.Core;
using Mars.Components.Layers;
using Mars.Interfaces.Annotations;
using Mars.Interfaces.Environments;
using SOHModel.Domain.Graph;
using SOHModel.Ferry.Station;

namespace SOHModel.Multimodal.Model;

public class DockWorkerSchedulerLayer : SchedulerLayer
{
    public DockWorkerSchedulerLayer(DockWorkerLayer dockWorkerLayer)
    {
        WorkerLayer = dockWorkerLayer;
    }


    public DockWorkerSchedulerLayer(DockWorkerLayer dockWorkerLayer, DataTable table) : base(table)
    {
        WorkerLayer = dockWorkerLayer;
    }

    [PropertyDescription] public DockWorkerLayer WorkerLayer { get; }

    [PropertyDescription] public FerryStationLayer StationLayer { get; set; }

    [PropertyDescription] public ISpatialGraphLayer Environment { get; set; }

    protected override void Schedule(SchedulerEntry dataRow)
    {
        Position? start = dataRow.SourceGeometry.RandomPositionFromGeometry();
        Position? goal = dataRow.TargetGeometry.RandomPositionFromGeometry();

        DockWorker dockWorker = new DockWorker
        {
            FerryStationLayer = StationLayer,
            EnvironmentLayer = Environment,
            StartPosition = start,
            GoalPosition = goal,
            TravelScheduleId = dataRow.Data.TryGetValue("id", out object? id) ? id.Value<int>() : 0
        };
        dockWorker.Init(WorkerLayer);

        if (dataRow.Data.TryGetValue("gender", out object? gender) && gender != null)
            dockWorker.Gender = gender.Value<GenderType>();
        if (dataRow.Data.TryGetValue("mass", out object? mass) && mass != null)
            dockWorker.Mass = mass.Value<double>();
        if (dataRow.Data.TryGetValue("perceptionInMeter", out object? perception) && perception != null)
            dockWorker.PerceptionInMeter = perception.Value<double>();

        WorkerLayer.Agents.Add(dockWorker.ID, dockWorker);
        RegisterAgent(WorkerLayer, dockWorker);
    }
}