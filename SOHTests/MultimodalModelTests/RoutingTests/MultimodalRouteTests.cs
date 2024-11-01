using System;
using Mars.Components.Environments;
using Mars.Interfaces.Environments;
using SOHModel.Multimodal.Routing;
using SOHTests.Commons.Environment;
using Xunit;

namespace SOHTests.MultimodalModelTests.RoutingTests;

public class MultimodalRouteTests
{
    private static Route GetRandomRoute(ISpatialGraphEnvironment environment)
    {
        ISpatialNode? start = environment.GetRandomNode();
        ISpatialNode? goal = environment.GetRandomNode();

        Route? randomRoute = environment.FindRoute(start, goal);

        int counter = 0;
        while (goal.Equals(start) || randomRoute == null)
        {
            if (counter++ > 100)
                throw new ApplicationException("Could not find random route. Counter exceeded 100 tries.");
            start = environment.GetRandomNode();
            goal = environment.GetRandomNode();
            randomRoute = environment.FindRoute(start, goal);
        }

        return randomRoute;
    }

    [Fact]
    public void AppendAndDeleteTail()
    {
        SpatialGraphEnvironment environment = new SpatialGraphEnvironment(ResourcesConstants.DriveGraphAltonaAltstadt);
        MultimodalRoute multimodalRoute = new MultimodalRoute
        {
            { GetRandomRoute(environment), ModalChoice.CyclingRentalBike },
            { GetRandomRoute(environment), ModalChoice.Walking },
            { GetRandomRoute(environment), ModalChoice.CyclingRentalBike }
        };
        Assert.Equal(3, multimodalRoute.Count);

        Assert.Equal(ModalChoice.CyclingRentalBike, multimodalRoute.CurrentModalChoice);
        multimodalRoute.Next();

        Assert.Equal(2, multimodalRoute.Count);
        Assert.Equal(ModalChoice.Walking, multimodalRoute.CurrentModalChoice);

        MultimodalRoute append = new MultimodalRoute
        {
            { GetRandomRoute(environment), ModalChoice.CarDriving },
            { GetRandomRoute(environment), ModalChoice.Walking },
            { GetRandomRoute(environment), ModalChoice.CarDriving }
        };

        multimodalRoute.AppendAndDeleteTail(append);
        Assert.Equal(3, multimodalRoute.Count);
        Assert.Equal(ModalChoice.CarDriving, multimodalRoute.CurrentModalChoice);

        multimodalRoute.Next();
        Assert.Equal(ModalChoice.Walking, multimodalRoute.CurrentModalChoice);

        multimodalRoute.Next();
        Assert.Equal(ModalChoice.CarDriving, multimodalRoute.CurrentModalChoice);
    }

    [Fact]
    public void AppendAtEnd()
    {
        SpatialGraphEnvironment environment = new SpatialGraphEnvironment(ResourcesConstants.DriveGraphAltonaAltstadt);
        MultimodalRoute multimodalRoute = new MultimodalRoute
        {
            { GetRandomRoute(environment), ModalChoice.CyclingRentalBike }
        };
        Assert.Equal(1, multimodalRoute.Count);
        Assert.Equal(0, multimodalRoute.PassedStops);
        Assert.Equal(ModalChoice.CyclingRentalBike, multimodalRoute.CurrentModalChoice);
        Assert.False(multimodalRoute.CurrentRoute.GoalReached);

        multimodalRoute.Next();
        Assert.Equal(1, multimodalRoute.Count);
        Assert.Equal(0, multimodalRoute.PassedStops);
        Assert.Equal(ModalChoice.CyclingRentalBike, multimodalRoute.CurrentModalChoice);
        Assert.False(multimodalRoute.CurrentRoute.GoalReached);

        MultimodalRoute append = new MultimodalRoute
        {
            { GetRandomRoute(environment), ModalChoice.CarDriving },
            { GetRandomRoute(environment), ModalChoice.Walking },
            { GetRandomRoute(environment), ModalChoice.CarDriving }
        };
        multimodalRoute.AppendAndDeleteTail(append);
        Assert.Equal(3, multimodalRoute.Count);
        Assert.Equal(ModalChoice.CarDriving, multimodalRoute.CurrentModalChoice);

        multimodalRoute.Next();
        Assert.Equal(ModalChoice.Walking, multimodalRoute.CurrentModalChoice);

        multimodalRoute.Next();
        Assert.Equal(ModalChoice.CarDriving, multimodalRoute.CurrentModalChoice);
    }

    [Fact]
    public void AppendToEmpty()
    {
        SpatialGraphEnvironment environment = new SpatialGraphEnvironment(ResourcesConstants.DriveGraphAltonaAltstadt);
        MultimodalRoute multimodalRoute = new MultimodalRoute();
        Assert.Equal(0, multimodalRoute.Count);

        MultimodalRoute append = new MultimodalRoute
        {
            { GetRandomRoute(environment), ModalChoice.CarDriving },
            { GetRandomRoute(environment), ModalChoice.Walking },
            { GetRandomRoute(environment), ModalChoice.CarDriving }
        };
        multimodalRoute.AppendAndDeleteTail(append);
        Assert.Equal(3, multimodalRoute.Count);
        Assert.Equal(ModalChoice.CarDriving, multimodalRoute.CurrentModalChoice);

        multimodalRoute.Next();
        Assert.Equal(ModalChoice.Walking, multimodalRoute.CurrentModalChoice);

        multimodalRoute.Next();
        Assert.Equal(ModalChoice.CarDriving, multimodalRoute.CurrentModalChoice);
    }

    [Fact]
    public void EmptyRouteHasMaxTravelTime()
    {
        MultimodalRoute multimodalRoute = new MultimodalRoute();
        Assert.Equal(int.MaxValue, multimodalRoute.ExpectedTravelTime());
    }

    [Fact]
    public void FindMainModalType()
    {
        FourNodeGraphEnv fourNodeGraphEnv = new FourNodeGraphEnv();
        ISpatialGraphEnvironment environment = fourNodeGraphEnv.GraphEnvironment;

        MultimodalRoute multimodalRoute = new MultimodalRoute();
        Assert.Equal(ModalChoice.Walking, multimodalRoute.MainModalChoice);

        Route? route1 = environment.FindRoute(fourNodeGraphEnv.Node1, fourNodeGraphEnv.Node2);
        Route? route2 = environment.FindRoute(fourNodeGraphEnv.Node2, fourNodeGraphEnv.Node3);
        Route? route3 = environment.FindRoute(fourNodeGraphEnv.Node3, fourNodeGraphEnv.Node4);

        multimodalRoute = new MultimodalRoute { { route1, ModalChoice.Walking } };
        Assert.Equal(ModalChoice.Walking, multimodalRoute.MainModalChoice);

        multimodalRoute.Add(route2, ModalChoice.CarDriving);
        Assert.Equal(ModalChoice.CarDriving, multimodalRoute.MainModalChoice);

        multimodalRoute.Add(route3, ModalChoice.Walking);
        Assert.Equal(ModalChoice.CarDriving, multimodalRoute.MainModalChoice);

        multimodalRoute = new MultimodalRoute
        {
            { route1, ModalChoice.Walking }, { route2, ModalChoice.CyclingRentalBike }, { route2, ModalChoice.Walking }
        };
        Assert.Equal(ModalChoice.CyclingRentalBike, multimodalRoute.MainModalChoice);
    }

    [Fact]
    public void NewMultimodalRouteHasReasonableParameters()
    {
        MultimodalRoute multimodalRoute = new MultimodalRoute();
        Assert.True(multimodalRoute.GoalReached);
        Assert.Empty(multimodalRoute.CurrentRoute);
        Assert.Equal(ModalChoice.Walking, multimodalRoute.CurrentModalChoice);

        //nothing changes
        multimodalRoute.Next();
        Assert.True(multimodalRoute.GoalReached);
        Assert.Empty(multimodalRoute.CurrentRoute);
        Assert.Equal(ModalChoice.Walking, multimodalRoute.CurrentModalChoice);
    }

    [Fact]
    public void SwitchThroughMultimodalRoute()
    {
        FourNodeGraphEnv fourNodeGraph = new FourNodeGraphEnv();
        ISpatialGraphEnvironment environment = fourNodeGraph.GraphEnvironment;
        Route? route1 = environment.FindRoute(fourNodeGraph.Node1, fourNodeGraph.Node2);
        Route? route2 = environment.FindRoute(fourNodeGraph.Node1, fourNodeGraph.Node3);
        Route? route3 = environment.FindRoute(fourNodeGraph.Node1, fourNodeGraph.Node4);

        MultimodalRoute multimodalRoute = new MultimodalRoute
        {
            { route1, ModalChoice.Walking }, { route2, ModalChoice.CarDriving }, { route3, ModalChoice.CoDriving }
        };
        Assert.Equal(3, multimodalRoute.Count);
        Assert.False(multimodalRoute.GoalReached);

        Assert.Equal(route1, multimodalRoute.CurrentRoute);
        Assert.Equal(ModalChoice.Walking, multimodalRoute.CurrentModalChoice);

        multimodalRoute.Next();
        Assert.Equal(route2, multimodalRoute.CurrentRoute);
        Assert.NotEqual(route1, multimodalRoute.CurrentRoute);
        Assert.Equal(ModalChoice.CarDriving, multimodalRoute.CurrentModalChoice);

        multimodalRoute.Next();
        Assert.Equal(route3, multimodalRoute.CurrentRoute);
        Assert.NotEqual(route2, multimodalRoute.CurrentRoute);
        Assert.Equal(ModalChoice.CoDriving, multimodalRoute.CurrentModalChoice);

        multimodalRoute.Next();
        Assert.Equal(route3, multimodalRoute.CurrentRoute);
        Assert.Equal(ModalChoice.CoDriving, multimodalRoute.CurrentModalChoice);
    }
}