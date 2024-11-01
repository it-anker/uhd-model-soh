using System.Data;
using Mars.Common;
using Mars.Common.Core;
using Mars.Components.Layers;
using Mars.Interfaces.Annotations;
using Mars.Interfaces.Environments;
using SOHModel.Multimodal.Layers;

namespace SOHModel.Multimodal.Model;

public class CitizenSchedulerLayer : SchedulerLayer
{
    private readonly CitizenLayer _citizenLayer;

    public CitizenSchedulerLayer(CitizenLayer citizenLayer)
    {
        _citizenLayer = citizenLayer;
    }

    public CitizenSchedulerLayer(CitizenLayer citizenLayer, DataTable table) : base(table)
    {
        _citizenLayer = citizenLayer;
    }

    [PropertyDescription] public MediatorLayer MediatorLayer { get; set; }

    protected override void Schedule(SchedulerEntry dataRow)
    {
        if (dataRow.SourceGeometry == null)
            throw new ArgumentException("No source geometry provided for citizen scheduling input");

        Position? source = dataRow.SourceGeometry.RandomPositionFromGeometry();

        bool isWorker = dataRow.Data.TryGetValue("worker", out object? worker) && worker.Value<bool>();
        bool isPartTimeWorker =
            dataRow.Data.TryGetValue("partTimeWorker", out object? partTime) && partTime.Value<bool>();


        Citizen citizen = new Citizen
        {
            StartPosition = source, Worker = isWorker, PartTimeWorker = isPartTimeWorker, MediatorLayer = MediatorLayer
        };
        citizen.Init(_citizenLayer);

        if (dataRow.Data.TryGetValue("gender", out object? gender)) citizen.Gender = gender.Value<GenderType>();
        if (dataRow.Data.TryGetValue("mass", out object? mass)) citizen.Mass = mass.Value<double>();
        if (dataRow.Data.TryGetValue("speed", out object? speed)) citizen.Velocity = speed.Value<double>();
        if (dataRow.Data.TryGetValue("height", out object? height)) citizen.Height = height.Value<double>();
        if (dataRow.Data.TryGetValue("width", out object? width)) citizen.Width = width.Value<double>();

        RegisterAgent(_citizenLayer, citizen);
    }
}