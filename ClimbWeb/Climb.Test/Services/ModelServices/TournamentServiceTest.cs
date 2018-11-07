using System.Linq;
using System.Threading.Tasks;
using Climb.Data;
using Climb.Exceptions;
using Climb.Services;
using Climb.Test.Utilities;
using NUnit.Framework;

namespace Climb.Test.Services.ModelServices
{
    [TestFixture]
    public class TournamentServiceTest
    {
        private TournamentService testObj;
        private ApplicationDbContext dbContext;

        [SetUp]
        public void SetUp()
        {
            dbContext = DbContextUtility.CreateMockDb();

            testObj = new TournamentService(dbContext);
        }

        [Test]
        public void Create_NoLeague_NotFoundException()
        {
            Assert.ThrowsAsync<NotFoundException>(() => testObj.Create(-1, null, "TestName"));
        }

        [Test]
        public void Create_NoSeason_NotFoundException()
        {
            var league = LeagueUtility.CreateLeague(dbContext);

            Assert.ThrowsAsync<NotFoundException>(() => testObj.Create(league.ID, -1, "TestName"));
        }

        [Test]
        public async Task Create_Season_AddCompetitors()
        {
            const int competitorCount = 4;
            var season = SeasonUtility.CreateSeason(dbContext, competitorCount).season;
            for(var i = 0; i < season.Participants.Count; i++)
            {
                season.Participants[i].Standing = i;
            }

            var tournament = await testObj.Create(season.LeagueID, season.ID, "TestName");

            Assert.AreEqual(competitorCount, tournament.TournamentUsers.Count);
            foreach(var participant in season.Participants)
            {
                var seed = tournament.TournamentUsers.First(tu => tu.SeasonLeagueUserID == participant.ID).Seed;
                Assert.AreEqual(participant.Standing, seed);
            }
        }
        
        // TODO: league and season don't match
    }
}