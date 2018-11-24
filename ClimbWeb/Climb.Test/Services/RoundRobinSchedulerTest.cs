using System;
using System.Linq;
using Climb.Data;
using Climb.Services;
using Climb.Test.Utilities;
using NUnit.Framework;

namespace Climb.Test.Services
{
    public class RoundRobinSchedulerTest
    {
        private RoundRobinScheduler testObj;
        private ApplicationDbContext dbContext;

        [SetUp]
        public void SetUp()
        {
            dbContext = DbContextUtility.CreateMockDb();

            testObj = new RoundRobinScheduler();
        }

        [TestCase(2, 1)]
        [TestCase(4, 6)]
        [TestCase(11, 55)]
        public void GenerateSchedule_Valid_CreateSets(int userCount, int setCount)
        {
            var season = SeasonUtility.CreateSeason(dbContext, userCount).season;

            var sets = testObj.GenerateSchedule(DateTime.MinValue, DateTime.MaxValue, season.Participants);

            Assert.AreEqual(setCount, sets.Count);
        }

        [TestCase(10)]
        [TestCase(15)]
        public void GenerateSchedule_Valid_EveryoneFightsEveryone(int userCount)
        {
            var season = SeasonUtility.CreateSeason(dbContext, userCount).season;

            var sets = testObj.GenerateSchedule(DateTime.MinValue, DateTime.MaxValue, season.Participants);

            Assert.IsTrue(season.Participants.All(slu =>
            {
                var fightCount = sets.Where(s => s
                        .IsPlaying(slu.LeagueUserID))
                    .Select(s => s.GetOpponentID(slu.LeagueUserID))
                    .Distinct()
                    .Count();
                return fightCount == userCount - 1;
            }));
        }

        [TestCase(4, 3)]
        [TestCase(5, 3)]
        [TestCase(4, 6)]
        [TestCase(5, 6)]
        public void GenerateSchedule_Valid_SpacesOutDueDates(int userCount, int days)
        {
            var startDate = DateTime.MinValue;
            var season = SeasonUtility.CreateSeason(dbContext, userCount, s =>
            {
                s.StartDate = startDate;
                s.EndDate = startDate.AddDays(days);
            }).season;

            var sets = testObj.GenerateSchedule(season.StartDate, season.EndDate, season.Participants);

            var roundCount = userCount - 1;
            var setsPerRound = userCount / 2;
            var daysPerRound = days / roundCount;
            for(var i = 0; i < roundCount; i++)
            {
                var dueDate = startDate.AddDays((i + 1) * daysPerRound);
                for(var j = 0; j < setsPerRound; j++)
                {
                    var setIndex = i * setsPerRound + j;
                    Assert.AreEqual(dueDate, sets[setIndex].DueDate, $"Round {i + 1}");
                }
            }
        }
    }
}