using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Climb.Data;
using Climb.Models;
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
        public async Task GenerateSchedule_Valid_CreateSets(int userCount, int setCount)
        {
            var season = SeasonUtility.CreateSeason(dbContext, userCount).season;
            season.Sets = new List<Set>();

            await testObj.GenerateScheduleAsync(season, dbContext);

            Assert.AreEqual(setCount, season.Sets.Count);
        }

        [TestCase(10)]
        [TestCase(15)]
        public async Task GenerateSchedule_Valid_EveryoneFightsEveryone(int userCount)
        {
            var season = SeasonUtility.CreateSeason(dbContext, userCount).season;
            season.Sets = new List<Set>();

            await testObj.GenerateScheduleAsync(season, dbContext);

            Assert.IsTrue(season.Participants.All(slu =>
            {
                var fightCount = season.Sets.Where(s => s
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
        public async Task GenerateSchedule_Valid_SpacesOutDueDates(int userCount, int days)
        {
            DateTime startDate = DateTime.MinValue;
            var season = SeasonUtility.CreateSeason(dbContext, userCount, s =>
            {
                s.StartDate = startDate;
                s.EndDate = startDate.AddDays(days);
                s.Sets = new List<Set>();
            }).season;

            var sets = await testObj.GenerateScheduleAsync(season, dbContext);

            var roundCount = userCount - 1;
            var setsPerRound = userCount / 2;
            var daysPerRound = days / roundCount;
            for(int i = 0; i < roundCount; i++)
            {
                var dueDate = startDate.AddDays((i + 1) * daysPerRound);
                for (int j = 0; j < setsPerRound; j++)
                {
                    var setIndex = i * setsPerRound + j;
                    Assert.AreEqual(dueDate, sets[setIndex].DueDate, $"Round {i + 1}");
                }
            }
        }
    }
}