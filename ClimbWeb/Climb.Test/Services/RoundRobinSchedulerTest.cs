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
    // TODO: Sets are spaced out.
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
    }
}