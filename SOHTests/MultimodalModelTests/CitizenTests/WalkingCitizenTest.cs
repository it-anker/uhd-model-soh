using System.Collections.Generic;
using System.Data;
using System.Linq;
using Mars.Common.Core;
using Mars.Components.Environments;
using Mars.Interfaces;
using Mars.Interfaces.Agents;
using Mars.Interfaces.Data;
using Mars.Interfaces.Environments;
using Mars.Numerics;
using SOHModel.Domain.Common;
using SOHModel.Domain.Graph;
using SOHModel.Multimodal.Layers;
using SOHModel.Multimodal.Model;
using SOHModel.Multimodal.Planning;
using SOHTests.Commons.Environment;
using SOHTests.Commons.Layer;
using Xunit;

namespace SOHTests.MultimodalModelTests.CitizenTests;

public class CitizenTest : IClassFixture<MediatorLayerAltonaAltstadtFixture>
{
    private const int SecondsPerDay = 24 * 60 * 60;
    private readonly SpatialGraphEnvironment _environment;
    private readonly CitizenLayer _layer;
    private readonly MediatorLayer _mediatorLayer;

    public CitizenTest(MediatorLayerAltonaAltstadtFixture mediatorLayerFixture)
    {
        _mediatorLayer = mediatorLayerFixture.MediatorLayer;
        _environment = new SpatialGraphEnvironment(ResourcesConstants.WalkGraphAltonaAltstadt);

        SimulationContext? context = SimulationContext.Start2020InSeconds;
        _mediatorLayer.Context = context;

        _layer = new CitizenLayer
        {
            MediatorLayer = _mediatorLayer,
            SpatialGraphMediatorLayer = new SpatialGraphMediatorLayer
            {
                Context = context,
                Environment = _environment
            }
        };
        _layer.InitLayer(new LayerInitData(context));
    }

    [Fact]
    public void CitizenSpawnsAtHome()
    {
        Position homePosition =
            _layer.MediatorLayer.GetNextPoiOfType(_environment.GetRandomNode().Position, OsmFeatureCodes.Buildings);
        Citizen citizen = new Citizen
        {
            Gender = GenderType.Male, Worker = true, PartTimeWorker = false, StartPosition = homePosition,
            MediatorLayer = _layer.MediatorLayer
        };
        citizen.Init(_layer);

        Assert.Equal(homePosition, citizen.Home.Position);
        Assert.InRange(Distance.Haversine(homePosition.PositionArray, citizen.Position.PositionArray), 0, 100);
    }

    [Fact]
    public void MoveFromHomeToWork()
    {
        ISpatialNode homeNode = _environment.NearestNode(Position.CreateGeoPosition(9.9571284, 53.5541156));
        ISpatialNode workNode = _environment.NearestNode(Position.CreateGeoPosition(9.948387, 53.5489888));
        Assert.NotEqual(homeNode.Position, workNode.Position);

        Citizen citizen = new Citizen
        {
            Gender = GenderType.Male,
            Worker = true, PartTimeWorker = false,
            StartPosition = homeNode.Position,
            EnvironmentLayer = _layer.SpatialGraphMediatorLayer,
            MediatorLayer = _layer.MediatorLayer
        };
        citizen.Init(_layer);
        Position startPosition = citizen.Position;
        Assert.InRange(citizen.Home.Position.DistanceInMTo(startPosition), 0, 23);
        Assert.NotEqual(workNode.Position, startPosition);

        citizen.ChangeWork(workNode.Position);
        Assert.Equal(workNode.Position, citizen.Work.Position);

        Assert.NotNull(citizen.Home);
        Assert.NotNull(citizen.Work);
        Assert.NotEqual(citizen.Home, citizen.Work);

        //does not move, because no next action chosen
        for (int tick = 0; tick < 10; tick++, _layer.Context.UpdateStep())
        {
            citizen.Tick();

            Assert.Equal(startPosition, citizen.Position);
            Assert.Equal(0, citizen.Velocity);
        }

        for (int tick = 0; tick < SecondsPerDay; tick++, _layer.Context.UpdateStep())
        {
            citizen.Tick();
            Trip dayPlanAction = citizen.Tour.Current;
            if (dayPlanAction is { TripReason: TripReason.Work } && citizen.GoalReached)
            {
                Assert.True(citizen.StoreTickResult);
                break;
            }
        }

        Assert.True(citizen.GoalReached);
        Assert.NotNull(citizen.Tour.Current);
        Assert.Equal(TripReason.Work, citizen.Tour.Current.TripReason);
        Assert.Equal(workNode.Position.Latitude, citizen.Position.Latitude, 7);
        Assert.Equal(workNode.Position.Longitude, citizen.Position.Longitude, 7);
        Assert.InRange(workNode.Position.DistanceInKmTo(citizen.Position), 0, 001);
    }

    [Fact]
    public void CitizenIsAtWorkAndHomeWithinFullDay()
    {
        Position homePosition = _layer.MediatorLayer.GetNextPoiOfType(_environment.GetRandomNode().Position,
            OsmFeatureCodes.Buildings, true, 1000);
        // homePosition = Position.CreateGeoPosition(9.9519875, 53.5453516);

        Position workPosition = _layer.MediatorLayer.GetNextPoiOfType(_environment.GetRandomNode().Position,
            OsmFeatureCodes.Buildings, true, 1000);
        // workPosition = Position.CreateGeoPosition(9.9338139, 53.5485157);

        while (homePosition.DistanceInKmTo(workPosition) < 1)
            workPosition = _layer.MediatorLayer.GetNextPoiOfType(_environment.GetRandomNode().Position,
                OsmFeatureCodes.Buildings, true, 1000);

        Assert.NotEqual(homePosition, workPosition);

        Citizen citizen = new Citizen
        {
            Gender = GenderType.Male,
            Worker = true,
            PartTimeWorker = false,
            StartPosition = homePosition,
            EnvironmentLayer = _layer.SpatialGraphMediatorLayer,
            MediatorLayer = _layer.MediatorLayer
        };

        citizen.Init(_layer);
        Assert.True(citizen.Worker);
        Assert.NotNull(citizen.Home);
        Assert.NotNull(citizen.Tour);

        citizen.ChangeWork(workPosition);
        Assert.NotEqual(citizen.Home, citizen.Work);

        Assert.Equal(homePosition, citizen.Home.Position);
        Assert.Equal(workPosition, citizen.Work.Position);

        Position? homeNodePosition = _environment.NearestNode(homePosition).Position;
        Position? workNodePosition = _environment.NearestNode(workPosition).Position;
        Assert.NotEqual(homeNodePosition, workNodePosition);

        bool wasAtHome = false;
        bool wasAtWork = false;

        for (int tick = 0; tick < SecondsPerDay; tick++, _layer.Context.UpdateStep())
        {
            citizen.Tick();
            wasAtHome |= citizen.Position.DistanceInMTo(homeNodePosition) < 10;
            wasAtWork |= citizen.Position.DistanceInMTo(workNodePosition) < 10;
        }

        Assert.True(wasAtHome);
        Assert.True(wasAtWork);
    }

    [Fact]
    public void StartDayAtHomeEndDayAtHome()
    {
        Citizen citizen = new Citizen
        {
            Gender = GenderType.Male,
            Worker = true,
            PartTimeWorker = false,
            EnvironmentLayer = _layer.SpatialGraphMediatorLayer,
            MediatorLayer = _layer.MediatorLayer
        };
        citizen.Init(_layer);
        Assert.InRange(citizen.Home.Position.DistanceInKmTo(citizen.Position), 0, 0.01);

        bool AtWork()
        {
            return (citizen.Tour.Current?.TripReason.Equals(TripReason.Work) ?? false) && citizen.GoalReached;
        }

        for (int tick = 0; tick < SecondsPerDay && !AtWork(); tick++, _layer.Context.UpdateStep())
            citizen.Tick();

        Assert.NotNull(citizen.Tour.Current);
        Assert.Equal(TripReason.Work, citizen.Tour.Current.TripReason);

        ISpatialNode workNode = _environment.NearestNode(citizen.Work.Position);
        Assert.InRange(workNode.Position.DistanceInKmTo(citizen.Position), 0, 0.01);

        bool AtHome()
        {
            Assert.NotNull(citizen.Tour.Current);
            return citizen.Tour.Current.TripReason.Equals(TripReason.HomeTime) && citizen.GoalReached;
        }

        for (int tick = 0; tick < SecondsPerDay && !AtHome(); tick++, _layer.Context.UpdateStep())
            citizen.Tick();

        Assert.NotNull(citizen.Tour.Current);
        Assert.Equal(TripReason.HomeTime, citizen.Tour.Current.TripReason);

        // At the end the agent is on a node.
        ISpatialNode homeNode = _environment.NearestNode(citizen.Home.Position);
        Assert.InRange(homeNode.Position.DistanceInKmTo(citizen.Position), 0, 0.01);
    }

    [Fact]
    public void TestCreateCitizensByScheduler()
    {
        DataTable table = new DataTable();

        table.Columns.Add("startTime");
        table.Columns.Add("endTime");
        table.Columns.Add("spawningIntervalInMinutes");
        table.Columns.Add("spawningAmount");
        table.Columns.Add("source");
        table.Columns.Add("destination");
        table.Columns.Add("workingKind");

        table.LoadDataRow(
            new object[]
                { "7:00", "18:00", 30, 1, "Point(9.95253 53.54907)", "Point(9.92812 53.52143)", "unemployed" },
            LoadOption.Upsert);
        table.LoadDataRow(
            new object[]
                { "5:00", "14:00", 60, 2, "Point(9.91582 53.54836)", "Point(9.92797 53.5067499)", "fullTimeWorker" },
            LoadOption.Upsert);
        table.LoadDataRow(
            new object[]
                { "16:00", "23:00", 60, 2, "Point(9.97033 53.54898)", "Point(9.949668 53.531397)", "partTimeWorker" },
            LoadOption.Upsert);
        table.LoadDataRow(
            new object[]
            {
                "9:00", "20:00", 10, 2, "Point(9.98911 53.54531)",
                "LineString(9.891536 53.534310, 9.97969 53.54480)", "partTimeWorker"
            },
            LoadOption.Upsert);
        table.LoadDataRow(
            new object[]
                { "6:00", "9:00", 10, 4, "Point(9.87707 53.53461)", "Point(9.97969 53.54480)", "fullTimeWorker" },
            LoadOption.Upsert);

        SimulationContext? context = SimulationContext.Start2020InSeconds;
        CitizenLayer citizenLayer = new CitizenLayer
        {
            MediatorLayer = _mediatorLayer,
            SpatialGraphMediatorLayer = new SpatialGraphMediatorLayer
            {
                Environment = new FourNodeGraphEnv().GraphEnvironment
            }
        };
        CitizenSchedulerLayer layer = new CitizenSchedulerLayer(citizenLayer, table)
        {
            MediatorLayer = _mediatorLayer
        };
        citizenLayer.Context = context;

        List<ITickClient> citizens = new List<ITickClient>();
        layer.InitLayer(new LayerInitData(), (_, client) => citizens.Add(client), (_, _) => { });

        layer.Context = context;
        Assert.NotNull(layer.SchedulingTable);
        Assert.NotNull(layer.TimeSeries);

        for (int i = 0; i < 54000; i++)
        {
            layer.PreTick();
            context.UpdateStep();
        }

        Assert.NotEmpty(citizens);
    }

    [Fact]
    public void TestInitializeSchedulerLayer()
    {
        DataTable table = new DataTable();

        table.Columns.Add("startTime");
        table.Columns.Add("endTime");
        table.Columns.Add("spawningIntervalInMinutes");
        table.Columns.Add("spawningAmount");
        table.Columns.Add("source");
        table.Columns.Add("destination");
        table.Columns.Add("workingKind");

        table.LoadDataRow(
            new object[]
                { "7:00", "18:00", 30, 1, "Point(9.95253 53.54907)", "Point(9.92812 53.52143)", "unemployed" },
            LoadOption.Upsert);
        table.LoadDataRow(
            new object[]
                { "5:00", "14:00", 60, 2, "Point(9.91582 53.54836)", "Point(9.92797 53.5067499)", "fullTimeWorker" },
            LoadOption.Upsert);
        table.LoadDataRow(
            new object[]
                { "16:00", "23:00", 60, 2, "Point(9.97033 53.54898)", "Point(9.949668 53.531397)", "partTimeWorker" },
            LoadOption.Upsert);
        table.LoadDataRow(
            new object[]
            {
                "9:00", "20:00", 10, 2, "Point(9.98911 53.54531)",
                "LineString(9.891536 53.534310, 9.97969 53.54480)", "partTimeWorker"
            },
            LoadOption.Upsert);
        table.LoadDataRow(
            new object[]
                { "6:00", "9:00", 10, 4, "Point(9.87707 53.53461)", "Point(9.97969 53.54480)", "fullTimeWorker" },
            LoadOption.Upsert);

        CitizenLayer citizenLayer = new CitizenLayer
        {
            MediatorLayer = _mediatorLayer
        };
        CitizenSchedulerLayer schedulerLayer = new CitizenSchedulerLayer(citizenLayer, table)
        {
            MediatorLayer = _mediatorLayer
        };

        schedulerLayer.InitLayer(new LayerInitData(), (_, _) => { }, (_, _) => { });

        Assert.NotNull(schedulerLayer.SchedulingTable);
        Assert.NotNull(schedulerLayer.AllDayTimeSeries);
        WorkingType[] workingKinds = schedulerLayer.AllDayTimeSeries.Values
            .Select(entry => entry.Data["workingKind"].Value<WorkingType>()).ToArray();
        Assert.Contains(WorkingType.Unemployed, workingKinds);
        Assert.Contains(WorkingType.FullTimeWorker, workingKinds);
        Assert.Contains(WorkingType.PartTimeWorker, workingKinds);
    }
}