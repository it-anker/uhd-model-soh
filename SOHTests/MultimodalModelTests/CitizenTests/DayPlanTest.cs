using System;
using System.Collections.Generic;
using System.Linq;
using Mars.Interfaces;
using SOHModel.Multimodal.Planning;
using Xunit;

namespace SOHTests.MultimodalModelTests.CitizenTests;

public class DayPlanTest
{
    private readonly DateTime _date;

    public DayPlanTest()
    {
        SimulationContext? context = SimulationContext.Start2020InSeconds;

        Assert.NotNull(context.StartTimePoint);
        _date = context.StartTimePoint.Value;
    }

    [Fact]
    public void Check24Hours()
    {
        for (int i = 0; i < 10000; i++)
        {
            AssertLastActionBeforeFirstActionPlus24H(
                DayPlanGenerator.CreateDayPlanForAgent(_date, true, false).ToList());
            AssertLastActionBeforeFirstActionPlus24H(
                DayPlanGenerator.CreateDayPlanForAgent(_date, true, true).ToList());
            AssertLastActionBeforeFirstActionPlus24H(
                DayPlanGenerator.CreateDayPlanForAgent(_date, false, false).ToList());
        }

        static void AssertLastActionBeforeFirstActionPlus24H(IReadOnlyCollection<Trip> dayPlanFullWorker)
        {
            Trip? firstAction = dayPlanFullWorker.FirstOrDefault();
            Trip? lastAction = dayPlanFullWorker.LastOrDefault();

            Assert.NotNull(firstAction);
            Assert.NotNull(lastAction);
            Assert.True(lastAction.StartTime <= firstAction.StartTime.AddHours(24));
        }
    }


    [Fact]
    public void TestDayPlanWithFixAppointment()
    {
        DateTime start = DateTime.Today.AddHours(8);

        IEnumerable<Trip> dayplan = DayPlanGenerator.CreateDayPlanForAgent(DateTime.Today,
            true,
            false,
            new Dictionary<TripReason, DateTime>
            {
                { TripReason.Work, DateTime.Today.AddHours(8) }
            });

        Assert.NotNull(dayplan);
        Assert.Contains(dayplan,
            action => action.TripReason == TripReason.Work && action.StartTime == start);
    }

    [Fact]
    public void CreateDayPlan()
    {
        //Generate 1000 day plans for every type of worker and check if star time is lower then next action start time         
        for (int i = 0; i < 10000; i++)
        {
            List<Trip> dayPlanFullWorker = DayPlanGenerator.CreateDayPlanForAgent(_date, true,
                false).ToList();
            Trip? fullWorkerAction = dayPlanFullWorker.FirstOrDefault();
            foreach (Trip action in dayPlanFullWorker)
            {
                Assert.NotNull(fullWorkerAction);
                Assert.True(fullWorkerAction.StartTime <= action.StartTime);
                fullWorkerAction = action;
            }

            List<Trip> dayPlanHalfWorker = DayPlanGenerator.CreateDayPlanForAgent(_date, true,
                true).ToList();
            Trip? halfWorkerAction = dayPlanHalfWorker.FirstOrDefault();
            foreach (Trip action in dayPlanHalfWorker)
            {
                Assert.NotNull(halfWorkerAction);
                Assert.True(halfWorkerAction.StartTime <= action.StartTime);
                halfWorkerAction = action;
            }

            List<Trip> dayPlanNoWorker = DayPlanGenerator.CreateDayPlanForAgent(_date, false,
                false).ToList();
            Trip? noWorkerAction = dayPlanNoWorker.FirstOrDefault();
            foreach (Trip action in dayPlanNoWorker)
            {
                Assert.NotNull(noWorkerAction);
                Assert.True(noWorkerAction.StartTime <= action.StartTime);
                noWorkerAction = action;
            }

            Assert.NotNull(dayPlanFullWorker);
            Assert.NotNull(dayPlanHalfWorker);
            Assert.NotNull(dayPlanNoWorker);

            int fullWorkerHasTwoWorkBlocks =
                dayPlanFullWorker.FindAll(action => action.TripReason == TripReason.Work).Count;
            int halfWorkerHasOneWorkBlock =
                dayPlanHalfWorker.FindAll(action => action.TripReason == TripReason.Work).Count;
            int noWorkerHasNoWorkBlock =
                dayPlanNoWorker.FindAll(action => action.TripReason == TripReason.Work).Count;

            Assert.Equal(2, fullWorkerHasTwoWorkBlocks);
            Assert.Equal(1, halfWorkerHasOneWorkBlock);
            Assert.Equal(0, noWorkerHasNoWorkBlock);
        }
    }

    [Fact]
    public void TestDayPlanRestrictionForWorker()
    {
        IEnumerable<Trip> plan1 = DayPlanGenerator.CreateDayPlanForAgent(DateTime.Today, true, false);
        IEnumerable<Trip> plan2 = DayPlanGenerator.CreateDayPlanForAgent(DateTime.Today, true, true);
        IEnumerable<Trip> plan3 = DayPlanGenerator.CreateDayPlanForAgent(DateTime.Today, false, false);


        Assert.Equal(2, plan1.Count(action => action.TripReason == TripReason.Work));
        Assert.Equal(1, plan2.Count(action => action.TripReason == TripReason.Work));
        Assert.Equal(0, plan3.Count(action => action.TripReason == TripReason.Work));
    }

    [Fact]
    public void NoRepeatingActionsCreated()
    {
        for (int i = 0; i < 100000; i++)
        {
            List<Trip> dayPlanFullWorker = DayPlanGenerator.CreateDayPlanForAgent(_date, true,
                false).ToList();
            Trip? previous = dayPlanFullWorker.FirstOrDefault();
            for (int j = 1; j < dayPlanFullWorker.Count; j++)
            {
                Assert.NotNull(previous);
                Trip current = dayPlanFullWorker.ElementAt(j);
                Assert.NotEqual(previous.TripReason, current.TripReason);
                previous = current;
            }

            List<Trip> dayPlanHalfWorker = DayPlanGenerator.CreateDayPlanForAgent(_date, true,
                true).ToList();
            previous = dayPlanHalfWorker.FirstOrDefault();
            for (int j = 1; j < dayPlanHalfWorker.Count; j++)
            {
                Assert.NotNull(previous);
                Trip current = dayPlanHalfWorker.ElementAt(j);
                Assert.NotEqual(previous.TripReason, current.TripReason);
                previous = current;
            }

            List<Trip> dayPlanNoWorker = DayPlanGenerator.CreateDayPlanForAgent(_date, false,
                false).ToList();
            previous = dayPlanNoWorker.FirstOrDefault();
            for (int j = 1; j < dayPlanNoWorker.Count; j++)
            {
                Assert.NotNull(previous);
                Trip current = dayPlanNoWorker.ElementAt(j);
                Assert.NotEqual(previous.TripReason, current.TripReason);
                previous = current;
            }
        }
    }
}