using System.Linq;
using System.Threading.Tasks;
using Climb.Data;
using Climb.Exceptions;
using Climb.Models;
using Climb.Services;
using Climb.Services.ModelServices;
using Climb.Test.Utilities;
using NSubstitute;
using NUnit.Framework;

namespace Climb.Test.Services.ModelServices
{
    [TestFixture]
    public class LeagueServiceTest
    {
        private const int StartingPoints = 2000;

        private LeagueService testObj;
        private ApplicationDbContext dbContext;
        private IPointService pointService;

        [SetUp]
        public void SetUp()
        {
            dbContext = DbContextUtility.CreateMockDb();
            pointService = Substitute.For<IPointService>();

            testObj = new LeagueService(dbContext, pointService);
        }

        [Test]
        public async Task Create_Valid_ReturnLeague()
        {
            var admin = DbContextUtility.AddNew<ApplicationUser>(dbContext);
            var game = DbContextUtility.AddNew<Game>(dbContext);

            var league = await testObj.Create("", game.ID, admin.Id);

            Assert.IsNotNull(league);
        }

        [Test]
        public async Task Create_Valid_AdminAdded()
        {
            var admin = DbContextUtility.AddNew<ApplicationUser>(dbContext);
            var game = DbContextUtility.AddNew<Game>(dbContext);

            var league = await testObj.Create("", game.ID, admin.Id);

            Assert.AreEqual(admin.Id, league.Members[0].UserID);
        }

        [Test]
        public void Create_NoAdmin_NotFound()
        {
            var game = DbContextUtility.AddNew<Game>(dbContext);

            Assert.ThrowsAsync<NotFoundException>(() => testObj.Create("", game.ID, ""));
        }

        [Test]
        public void Create_NoGame_NotFound()
        {
            Assert.ThrowsAsync<NotFoundException>(() => testObj.Create("", 0, ""));
        }

        [Test]
        public void Create_NameTaken_Conflict()
        {
            var league = LeagueUtility.CreateLeague(dbContext);

            Assert.ThrowsAsync<ConflictException>(() => testObj.Create(league.Name, league.GameID, ""));
        }

        [Test]
        public async Task Join_NewUser_CreateLeagueUser()
        {
            var user = DbContextUtility.AddNew<ApplicationUser>(dbContext);
            var league1 = LeagueUtility.CreateLeague(dbContext);
            var league2 = LeagueUtility.CreateLeague(dbContext);

            await testObj.Join(league1.ID, user.Id);
            var leagueUser = await testObj.Join(league2.ID, user.Id);

            Assert.AreEqual(user.Id, leagueUser.UserID);
            Assert.AreEqual(league2.ID, leagueUser.LeagueID);
        }

        [Test]
        public async Task Join_OldUser_HasLeftFalse()
        {
            var league = LeagueUtility.CreateLeague(dbContext);
            var user = DbContextUtility.AddNew<ApplicationUser>(dbContext);
            LeagueUser oldLeagueUser = CreateOldLeagueUser(league, user);

            var leagueUser = await testObj.Join(league.ID, user.Id);

            Assert.IsFalse(leagueUser.HasLeft);
            Assert.AreEqual(oldLeagueUser.ID, leagueUser.ID);
        }

        [Test]
        public async Task Join_NewUser_UpdateDisplayName()
        {
            var league = LeagueUtility.CreateLeague(dbContext);
            var user = DbContextUtility.AddNew<ApplicationUser>(dbContext, u => u.UserName = "bob");

            var leagueUser = await testObj.Join(league.ID, user.Id);

            Assert.AreEqual(user.UserName, leagueUser.DisplayName);
        }

        [Test]
        public async Task Join_OldUser_UpdateDisplayName()
        {
            var league = LeagueUtility.CreateLeague(dbContext);
            var user = DbContextUtility.AddNew<ApplicationUser>(dbContext, u => u.UserName = "bob");
            CreateOldLeagueUser(league, user);
            user.UserName = "bob";
            dbContext.Update(user);
            dbContext.SaveChanges();

            var leagueUser = await testObj.Join(league.ID, user.Id);
            Assert.AreEqual(user.UserName, leagueUser.DisplayName);
        }

        [Test]
        public void Join_NoLeague_NotFound()
        {
            var user = DbContextUtility.AddNew<ApplicationUser>(dbContext);

            Assert.ThrowsAsync<NotFoundException>(() => testObj.Join(0, user.Id));
        }

        [Test]
        public void Join_NoUser_NotFound()
        {
            var league = LeagueUtility.CreateLeague(dbContext);

            Assert.ThrowsAsync<NotFoundException>(() => testObj.Join(league.ID, ""));
        }

        [Test]
        public void UpdateStandings_NoLeague_NotFoundException()
        {
            Assert.ThrowsAsync<NotFoundException>(() => testObj.UpdateStandings(0));
        }

        [Test]
        public async Task UpdateStandings_UniquePoints_NoTies()
        {
            var league = CreateLeague(10);
            league.SetsTillRank = 0;
            for(var i = 0; i < league.Members.Count; i++)
            {
                league.Members[i].Points = i;
            }

            await testObj.UpdateStandings(league.ID);

            var members = league.Members.OrderBy(lu => lu.Rank).ToList();

            for(var i = 0; i < members.Count; ++i)
            {
                Assert.AreEqual(i + 1, members[i].Rank);
            }
        }

        [Test]
        public async Task UpdateStandings_SharedPoints_CorrectlySkipPlace()
        {
            var league = CreateLeague(3);
            league.SetsTillRank = 0;
            league.Members[0].Points = 2;
            league.Members[1].Points = 2;
            league.Members[2].Points = 1;

            await testObj.UpdateStandings(league.ID);

            var members = league.Members.OrderBy(lu => lu.Rank).ToList();

            Assert.AreEqual(1, members[0].Rank);
            Assert.AreEqual(1, members[1].Rank);
            Assert.AreEqual(3, members[2].Rank);
        }

        [Test]
        public async Task UpdateStandings_NewcomerHasRank_RankSetTo0()
        {
            var league = CreateLeague(1);
            league.Members[0].Points = 2;
            league.Members[0].Rank = 2;

            await testObj.UpdateStandings(league.ID);

            Assert.AreEqual(0, league.Members[0].Rank);
        }

        [Test]
        public async Task UpdateStandings_Newcomer_PointsUpdated()
        {
            var league = CreateLeague(2);
            var player1 = league.Members[0];
            var originalPoints = player1.Points;

            var set = SetUtility.Create(dbContext, player1.ID, league.Members[1].ID, league.ID);
            DbContextUtility.UpdateAndSave(dbContext, set, () => { set.IsComplete = true; });
            
            pointService.CalculatePointDeltas(0, 0, false).ReturnsForAnyArgs((1, -1));

            await testObj.UpdateStandings(league.ID);

            Assert.AreNotEqual(originalPoints, player1.Points);
        }

        [Test]
        public async Task UpdateStandings_HasNewcomers_NewcomersSkippedWhenRanking()
        {
            var league = CreateLeague(4);

            dbContext.UpdateRange(league.Members);
            for(int i = 0; i < league.Members.Count; i++)
            {
                league.Members[i].Points = i;
                league.Members[i].SetCount = 10;
                league.Members[i].IsNewcomer = false;
            }
            league.Members[1].SetCount = 0;
            league.Members[1].IsNewcomer = true;
            dbContext.SaveChanges();

            await testObj.UpdateStandings(league.ID);

            var members = league.Members.OrderBy(lu => lu.Rank).ToArray();
            Assert.AreEqual(0, members[0].Rank);
            Assert.AreEqual(1, members[1].Rank);
            Assert.AreEqual(2, members[2].Rank);
            Assert.AreEqual(3, members[3].Rank);
        }

        [Test]
        public async Task TakeSnapshots_NewcomerHasEnoughSets_NewcomerStatusRemoved()
        {
            var league = CreateLeague(1);
            league.SetsTillRank = 2;
            var player = league.Members[0];
            player.SetCount = 2;

            await testObj.TakeSnapshots(league.ID);

            Assert.IsFalse(player.IsNewcomer);
        }

        [Test]
        public void TakeSnapshots_NoLeague_NotFound()
        {
            Assert.ThrowsAsync<NotFoundException>(() => testObj.TakeSnapshots(0));
        }

        [Test]
        public async Task TakeSnapshots_Season_ReturnSnapshots()
        {
            var season = CreateSeason(10);
            CreateSets(season.League, season);

            var snapshots = await testObj.TakeSnapshots(season.LeagueID);

            Assert.AreEqual(season.League.Members.Count, snapshots.Count);
        }

        [Test]
        public async Task TakeSnapshots_SeasonAndExhibition_ReturnSnapshots()
        {
            var season = CreateSeason(10);
            CreateSets(season.League, null);
            CreateSets(season.League, season);

            var snapshots = await testObj.TakeSnapshots(season.LeagueID);

            Assert.AreEqual(season.League.Members.Count, snapshots.Count);
        }

        [Test]
        public async Task TakeSnapshots_HasSnapshot_SnapshotIsCorrect()
        {
            const int deltaRank = 4;
            const int deltaPoints = -50;
            var league = LeagueUtility.CreateLeague(dbContext);
            var member = LeagueUtility.AddUsersToLeague(league, 1, dbContext)[0];
            member.Rank = 14;
            member.Points = 2000;

            DbContextUtility.AddNew<RankSnapshot>(dbContext, rs =>
            {
                rs.LeagueUserID = member.ID;
                rs.Rank = member.Rank + deltaRank;
                rs.Points = member.Points + deltaPoints;
            });

            var snapshot = (await testObj.TakeSnapshots(league.ID))[0];

            Assert.AreEqual(member.Rank, snapshot.Rank, "Rank");
            Assert.AreEqual(member.Points, snapshot.Points, "Points");
            Assert.AreEqual(-deltaRank, snapshot.DeltaRank, "Delta Rank");
            Assert.AreEqual(-deltaPoints, snapshot.DeltaPoints, "Delta Points");
        }

        #region Helpers
        private Season CreateSeason(int memberCount)
        {
            var league = CreateLeague(memberCount);
            for(var i = 0; i < league.Members.Count; i++)
            {
                var member = league.Members[i];
                member.Points = StartingPoints - i;
                member.Rank = i + 1;
            }

            var season = DbContextUtility.AddNew<Season>(dbContext, s => s.League = league);
            return season;
        }

        private League CreateLeague(int memberCount)
        {
            var league = LeagueUtility.CreateLeague(dbContext);
            LeagueUtility.AddUsersToLeague(league, memberCount, dbContext);
            return league;
        }

        private void CreateSets(League league, Season season)
        {
            var firstMember = league.Members[0];
            for (var i = 1; i < league.Members.Count; i++)
            {
                var nextMember = league.Members[i].ID;
                var set = SetUtility.Create(dbContext, firstMember.ID, nextMember, league.ID, season);
                set.IsComplete = true;
                set.Player1Score = 2;
                set.Player2Score = 1;
            }
        }

        private LeagueUser CreateOldLeagueUser(League league, ApplicationUser user)
        {
            var oldLeagueUser = new LeagueUser(league.ID, user.Id) { HasLeft = true };
            dbContext.LeagueUsers.Add(oldLeagueUser);
            dbContext.SaveChanges();
            return oldLeagueUser;
        } 
        #endregion
    }
}