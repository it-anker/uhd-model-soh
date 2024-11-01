using Mars.Interfaces.Environments;
using SOHModel.Multimodal.Multimodal;
using SOHTests.Commons.Agent;
using SOHTests.Commons.Environment;
using SOHTests.Commons.Layer;
using Xunit;

namespace SOHTests.MultimodalModelTests.MultimodalWalkingTests;

public class PedestrianFourNodeEnvTests
{
    [Fact]
    public void GoalReachedByWalk()
    {
        FourNodeGraphEnv fourNodeGraphEnv = new FourNodeGraphEnv();
        TestMultimodalLayer layer = new TestMultimodalLayer(fourNodeGraphEnv.GraphEnvironment);

        Position start = FourNodeGraphEnv.Node1Pos;
        Position goal = FourNodeGraphEnv.Node4Pos;

        TestMultiCapableAgent agent = new TestMultiCapableAgent
        {
            StartPosition = start,
            GoalPosition = goal,
            ModalChoice = ModalChoice.Walking
        };
        agent.Init(layer);

        Assert.Equal(Whereabouts.Offside, agent.Whereabouts);
        for (int tick = 0; tick < 1000 && !agent.GoalReached; tick++, layer.Context.UpdateStep())
        {
            agent.Tick();
            if (!agent.GoalReached) Assert.Equal(Whereabouts.Sidewalk, agent.Whereabouts);
        }

        Assert.True(agent.GoalReached);
        Assert.Equal(Whereabouts.Offside, agent.Whereabouts);

        Assert.Equal(goal, agent.Position);
    }

    [Fact]
    public void StartIsGoal()
    {
        FourNodeGraphEnv fourNodeGraphEnv = new FourNodeGraphEnv();
        TestMultimodalLayer layer = new TestMultimodalLayer(fourNodeGraphEnv.GraphEnvironment);

        Position? start = fourNodeGraphEnv.Node2.Position;
        Position? goal = fourNodeGraphEnv.Node2.Position;

        TestMultiCapableAgent agent = new TestMultiCapableAgent
        {
            StartPosition = start,
            GoalPosition = goal,
            ModalChoice = ModalChoice.Walking
        };
        agent.Init(layer);

        Assert.Equal(Whereabouts.Offside, agent.Whereabouts);
        for (int tick = 0;
             tick < 1000 && !agent.GoalReached;
             tick++, layer.Context.UpdateStep()) agent.Tick();

        Assert.Equal(Whereabouts.Offside, agent.Whereabouts);

        Assert.True(agent.GoalReached);
        Assert.Equal(goal, agent.Position);
    }
}