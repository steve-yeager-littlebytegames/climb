using System.Linq;
using System.Threading.Tasks;
using Climb.Data;
using Climb.Exceptions;
using Climb.Services;
using Climb.Test.Utilities;
using NSubstitute;
using NUnit.Framework;

namespace Climb.Test.Services.ModelServices
{
    // TODO: league and season don't match
    [TestFixture]
    public class TournamentServiceTest
    {
        private TournamentService testObj;
        private ApplicationDbContext dbContext;
        private IBracketGenerator bracketGenerator;

        [SetUp]
        public void SetUp()
        {
            dbContext = DbContextUtility.CreateMockDb();
            bracketGenerator = Substitute.For<IBracketGenerator>();

            testObj = new TournamentService(dbContext, bracketGenerator);
        }

        [Test]
        public void Create_NoLeague_NotFoundException()
        {
            Assert.ThrowsAsync<NotFoundException>(() => testObj.Create(-1, null, "TestName"));
        }

        [Test]
        public void Create_NoSeason_NotFoundException()
        {
            var league = dbContext.CreateLeague();

            Assert.ThrowsAsync<NotFoundException>(() => testObj.Create(league.ID, -1, "TestName"));
        }

        [Test]
        public async Task Create_Season_AddCompetitors()
        {
            const int competitorCount = 4;
            var season = dbContext.CreateSeason(competitorCount).season;
            dbContext.UpdateAndSave(season, () =>
            {
                for(var i = 0; i < season.Participants.Count; i++)
                {
                    season.Participants[i].Standing = i;
                }
            });

            var tournament = await testObj.Create(season.LeagueID, season.ID, "TestName");

            Assert.AreEqual(competitorCount, tournament.TournamentUsers.Count);
            foreach(var participant in season.Participants)
            {
                var seed = tournament.TournamentUsers.First(tu => tu.SeasonLeagueUserID == participant.ID).Seed;
                Assert.AreEqual(participant.Standing, seed);
            }
        }

        [Test]
        public void Join_NoTournament_NotFoundException()
        {
            Assert.ThrowsAsync<NotFoundException>(() => testObj.Join(-1, ""));
        }

        [Test]
        public void Join_NoLeagueUser_NotFoundException()
        {
            var tournament = dbContext.CreateTournament();

            Assert.ThrowsAsync<NotFoundException>(() => testObj.Join(tournament.ID, ""));
        }

        [Test]
        public async Task Join_NewUser_AddTournamentUser()
        {
            var tournament = dbContext.CreateTournament();
            var member = dbContext.AddUsersToLeague(tournament.League, 1)[0];

            var competitor = await testObj.Join(tournament.ID, member.UserID);

            Assert.IsNotNull(competitor);
        }

        [Test]
        public async Task Join_AlreadyJoined_ReturnTournamentUser()
        {
            var tournament = dbContext.CreateTournament();
            var member = dbContext.AddUsersToLeague(tournament.League, 1)[0];

            await testObj.Join(tournament.ID, member.UserID);
            var competitor = await testObj.Join(tournament.ID, member.UserID);

            Assert.IsNotNull(competitor);
        }

        // TODO: Get season leagueuser
    }
}