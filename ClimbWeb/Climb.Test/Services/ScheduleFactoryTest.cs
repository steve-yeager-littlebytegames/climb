using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Climb.Data;
using Climb.Models;
using Climb.Services;
using Climb.Test.Utilities;
using NUnit.Framework;

namespace Climb.Test.Services
{
    // TODO: Already started.
    // TODO: Already ended.
    [TestFixture]
    public class ScheduleFactoryTest
    {
        private class FakeScheduler : ScheduleFactory
        {
            public List<Set> generatedSets;

            protected override List<Set> GenerateScheduleInternal(Season season)
            {
                return generatedSets;
            }
        }

        private FakeScheduler testObj;
        private ApplicationDbContext dbContext;

        [SetUp]
        public void SetUp()
        {
            dbContext = DbContextUtility.CreateMockDb();

            testObj = new FakeScheduler();
        }

        [TestCase(0)]
        [TestCase(1)]
        public void GenerateSchedule_NotEnoughParticipants_Exception(int userCount)
        {
            var league = LeagueUtility.CreateLeague(dbContext);
            LeagueUtility.AddUsersToLeague(league, userCount, dbContext);
            var season = SeasonUtility.CreateSeason(dbContext, userCount).season;

            Assert.ThrowsAsync<InvalidOperationException>(() => testObj.GenerateScheduleAsync(season, dbContext));
        }

        [Test]
        public async Task GenerateSchedule_AlreadyHasSets_ClearsOldSets()
        {
            var (season, participants) = SeasonUtility.CreateSeason(dbContext, 2);

            var oldSet = SetUtility.Create(dbContext, participants[0].ID, participants[1].ID, season.LeagueID);
            season.Sets = new List<Set> {oldSet};

            var newSet = new Set
            {
                LeagueID = season.LeagueID,
                SeasonID = season.ID,
                DueDate = DateTime.Now.AddDays(1),
                Player1ID = participants[0].ID,
                Player2ID = participants[1].ID,
            };
            testObj.generatedSets = new List<Set> {newSet};

            await testObj.GenerateScheduleAsync(season, dbContext);

            Assert.AreEqual(1, season.Sets.Count);
        }
    }
}