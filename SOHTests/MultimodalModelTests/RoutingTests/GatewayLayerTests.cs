using System;
using Mars.Components.Environments;
using Mars.Interfaces.Data;
using Mars.Interfaces.Environments;
using SOHModel.Multimodal.Routing;
using Xunit;

namespace SOHTests.MultimodalModelTests.RoutingTests;

public class GatewayLayerTests
{
    private readonly SpatialGraphEnvironment _environment;
    private readonly GatewayLayer _gatewayLayer;

    public GatewayLayerTests()
    {
        _environment = new SpatialGraphEnvironment(ResourcesConstants.WalkGraphAltonaAltstadt);
        _gatewayLayer = new GatewayLayer(_environment);
        _gatewayLayer.InitLayer(new LayerInitData
        {
            LayerInitConfig = { File = ResourcesConstants.RailroadStations }
        });
    }

    [Fact]
    public void FindGoalWithinEnv()
    {
        Position? start = Position.CreateGeoPosition(9.9460806, 53.5525467); //Schomburgstr/Hospitalstr
        Position? goal = Position.CreateGeoPosition(9.936516, 53.547820); //Königstr/Nordelbische Kirchenbib
        // Assert.True(_environment.BoundingBox.Envelope.Contains(start.PositionArray));

        Position validatedGoal = _gatewayLayer.Validate(start, goal).Item2;

        Assert.Equal(goal, validatedGoal);

        ISpatialNode startNode = _environment.NearestNode(start);
        ISpatialNode goalNode = _environment.NearestNode(validatedGoal);
        Route route = _environment.FindShortestRoute(startNode, goalNode);
        Assert.NotNull(route);
        Assert.NotEmpty(route);
    }

    [Fact]
    public void FindExitPointWithinWalkingDistanceToGoal()
    {
        Position? start = Position.CreateGeoPosition(9.9460806, 53.5525467);
        Position? goal = Position.CreateGeoPosition(9.9672284, 53.5573791);

        Position validatedGoal = _gatewayLayer.Validate(start, goal).Item2;

        Position? expectedGoal = Position.CreateGeoPosition(9.9590506, 53.5585846);

        Assert.Equal(expectedGoal, validatedGoal);
        Assert.InRange(goal.DistanceInKmTo(validatedGoal), 0, 1);
    }

    [Fact]
    public void FindEntryPointWithinWalkingDistanceToStart()
    {
        Position? start = Position.CreateGeoPosition(9.9672284, 53.5573791);
        Position? goal = Position.CreateGeoPosition(9.9460806, 53.5525467);

        Position validatedGoal = _gatewayLayer.Validate(start, goal).Item1;

        Position? expectedGoal = Position.CreateGeoPosition(9.9590506, 53.5585846);

        Assert.Equal(expectedGoal, validatedGoal);
        Assert.InRange(start.DistanceInKmTo(validatedGoal), 0, 1);
    }

    [Fact]
    public void FindExitPointOverGateway()
    {
        Position? start = Position.CreateGeoPosition(9.9460806, 53.5525467);
        Position? goal = Position.CreateGeoPosition(9.88361, 53.55891);

        Position gatewayPosition = _gatewayLayer.Validate(start, goal).Item2;

        Position? railStation = Position.CreateGeoPosition(9.944125, 53.547752);
        Position? railStationNodePosition = _environment.NearestNode(railStation).Position;

        Assert.InRange(goal.DistanceInKmTo(gatewayPosition), 1, 10);
        Assert.Equal(railStationNodePosition, gatewayPosition);
    }

    [Fact]
    public void FindEntryPointOverGateway()
    {
        Position? start = Position.CreateGeoPosition(9.88361, 53.55891); //S-Bahn Othmarschen
        Position? goal = Position.CreateGeoPosition(9.9460806, 53.5525467); //Schomburgstr/Hospitalstr

        Position gatewayPosition = _gatewayLayer.Validate(start, goal).Item1;

        Position? railStation = Position.CreateGeoPosition(9.944125, 53.547752); //S-Bahn Königstr
        Position? railStationNodePosition = _environment.NearestNode(railStation).Position;

        Assert.InRange(start.DistanceInKmTo(gatewayPosition), 1, 10);
        Assert.Equal(railStationNodePosition, gatewayPosition);
    }

    [Fact]
    public void StartAndGoalOutsideEnvironment()
    {
        Position? start = Position.CreateGeoPosition(9.9675872, 53.5614485);
        Position? goal = Position.CreateGeoPosition(9.9672284, 53.5573791);

        Position validatedGoal = _gatewayLayer.Validate(start, goal).Item2;

        Assert.Equal(goal, validatedGoal);
    }

    [Fact]
    public void GoalFarOutsideEnvironmentButLayerNotInitialized()
    {
        Position? start = Position.CreateGeoPosition(9.9460806, 53.5525467);
        Position? goal = Position.CreateGeoPosition(9.88361, 53.55891);

        SpatialGraphEnvironment environment = new SpatialGraphEnvironment(ResourcesConstants.WalkGraphAltonaAltstadt);
        GatewayLayer gatewayLayer = new GatewayLayer(environment);
        Assert.Throws<ApplicationException>(() => gatewayLayer.Validate(start, goal));
    }
}