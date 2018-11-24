using System;
using System.Linq;
using System.Threading.Tasks;
using Climb.Data;
using Climb.Exceptions;
using Climb.Models;
using Climb.Services;
using Climb.Services.ModelServices;
using Climb.Test.Utilities;
using MoreLinq;
using NSubstitute;
using NUnit.Framework;

namespace Climb.Test.Services.ModelServices
{
    [TestFixture]
    public class LeagueServiceTest
    {
        private LeagueService testObj;
        private ApplicationDbContext dbContext;
        private IPointService pointService;
        private ISeasonService seasonService;
        private ISetService setService;

        [SetUp]
        public void SetUp()
        {
            dbContext = DbContextUtility.CreateMockDb();
            pointService = Substitute.For<IPointService>();
            seasonService = Substitute.For<ISeasonService>();
            setService = Substitute.For<ISetService>();
            var dateService = Substitute.For<IDateService>();

            testObj = new LeagueService(dbContext, pointService, seasonService, setService, dateService);
        }

        [Test]
        public async Task Create_Valid_ReturnLeague()
        {
            var admin = dbContext.AddNew<ApplicationUser>();
            var game = dbContext.AddNew<Game>();

            var league = await testObj.Create("", game.ID, admin.Id);

            Assert.IsNotNull(league);
        }

        [Test]
        public async Task Create_Valid_AdminAdded()
        {
            var admin = dbContext.AddNew<ApplicationUser>();
            var game = dbContext.AddNew<Game>();

            var league = await testObj.Create("", game.ID, admin.Id);

            Assert.AreEqual(admin.Id, league.Members[0].UserID);
        }

        [Test]
        public void Create_NoAdmin_NotFound()
        {
            var game = dbContext.AddNew<Game>();

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
            var user = dbContext.AddNew<ApplicationUser>();
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
            var user = dbContext.AddNew<ApplicationUser>();
            var oldLeagueUser = CreateOldLeagueUser(league, user);

            var leagueUser = await testObj.Join(league.ID, user.Id);

            Assert.IsFalse(leagueUser.HasLeft);
            Assert.AreEqual(oldLeagueUser.ID, leagueUser.ID);
        }

        [Test]
        public async Task Join_NewUser_UpdateDisplayName()
        {
            var league = LeagueUtility.CreateLeague(dbContext);
            var user = dbContext.AddNew<ApplicationUser>(u => u.UserName = "bob");

            var leagueUser = await testObj.Join(league.ID, user.Id);

            Assert.AreEqual(user.UserName, leagueUser.DisplayName);
        }

        [Test]
        public async Task Join_OldUser_UpdateDisplayName()
        {
            var league = LeagueUtility.CreateLeague(dbContext);
            var user = dbContext.AddNew<ApplicationUser>(u => u.UserName = "bob");
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
            var user = dbContext.AddNew<ApplicationUser>();

            Assert.ThrowsAsync<NotFoundException>(() => testObj.Join(0, user.Id));
        }

        [Test]
        public void Join_NoUser_NotFound()
        {
            var league = LeagueUtility.CreateLeague(dbContext);

            Assert.ThrowsAsync<NotFoundException>(() => testObj.Join(league.ID, ""));
        }

        [Test]
        public async Task Join_NewUser_GetStartingPoints()
        {
            var user = dbContext.AddNew<ApplicationUser>();
            var league = LeagueUtility.CreateLeague(dbContext);

            var leagueUser = await testObj.Join(league.ID, user.Id);

            Assert.AreEqual(League.StartingPoints, leagueUser.Points);
        }

        [Test]
        public async Task Join_OldUser_KeepsPoints()
        {
            const int userPoints = League.StartingPoints - 1;

            var user = dbContext.AddNew<ApplicationUser>();
            var league = LeagueUtility.CreateLeague(dbContext);

            var leagueUser = await testObj.Join(league.ID, user.Id);
            dbContext.UpdateAndSave(leagueUser, lu =>
            {
                lu.Points = userPoints;
                lu.HasLeft = true;
            });
            leagueUser = await testObj.Join(league.ID, user.Id);

            Assert.AreEqual(userPoints, leagueUser.Points);
        }

        [Test]
        public async Task Leave_IsMember_HasLeftTrue()
        {
            var league = LeagueUtility.CreateLeague(dbContext, 1);
            var member = league.Members[0];

            member = await testObj.Leave(member.ID);

            Assert.IsTrue(member.HasLeft);
        }

        [Test]
        public async Task Leave_HasLeft_NoException()
        {
            var league = LeagueUtility.CreateLeague(dbContext, 1);
            var member = league.Members[0];
            dbContext.UpdateAndSave(member, lu => lu.HasLeft = true);

            await testObj.Leave(member.ID);

            Assert.Pass();
        }

        [Test]
        public void Leave_NoMember_NotFoundException()
        {
            Assert.ThrowsAsync<NotFoundException>(() => testObj.Leave(-1));
        }

        [Test]
        public async Task Leave_InSeasons_LeaveSeason()
        {
            var league = LeagueUtility.CreateLeague(dbContext, 1);
            var member = league.Members[0];

            var season = SeasonUtility.CreateSeason(dbContext, 0).season;
            var participant = SeasonUtility.AddParticipants(dbContext, season, member)[0];

            var seasonCompleted = SeasonUtility.CreateSeason(dbContext, 0, s => s.IsComplete = true).season;
            var participantCompleted = SeasonUtility.AddParticipants(dbContext, seasonCompleted, member)[0];

            await testObj.Leave(member.ID);

#pragma warning disable 4014
            seasonService.Received(1).LeaveAsync(participant.ID);
            seasonService.DidNotReceive().LeaveAsync(participantCompleted.ID);
#pragma warning restore 4014
        }

        [Test]
        public async Task Leave_IsChallenged_RequestDeclined()
        {
            var league = LeagueUtility.CreateLeague(dbContext, 2);
            var member = league.Members[0];
            var opponent = league.Members[1];
            var request = dbContext.AddNew<SetRequest>(sr =>
            {
                sr.LeagueID = league.ID;
                sr.ChallengedID = member.ID;
                sr.RequesterID = opponent.ID;
            });

            await testObj.Leave(member.ID);

#pragma warning disable 4014
            setService.Received(1).RespondToSetRequestAsync(request.ID, false);
#pragma warning restore 4014
        }

        [Test]
        public async Task Leave_IsRequester_RequestDeleted()
        {
            var league = LeagueUtility.CreateLeague(dbContext, 2);
            var member = league.Members[0];
            var opponent = league.Members[1];
            dbContext.AddNew<SetRequest>(sr =>
            {
                sr.LeagueID = league.ID;
                sr.ChallengedID = opponent.ID;
                sr.RequesterID = member.ID;
            });

            await testObj.Leave(member.ID);

            Assert.IsEmpty(dbContext.SetRequests.ToArray());
        }

        [Test]
        public async Task Leave_HasNonSeasonSet_SetDeleted()
        {
            var league = LeagueUtility.CreateLeague(dbContext, 2);
            var member = league.Members[0];
            var opponent = league.Members[1];
            SetUtility.Create(dbContext, member.ID, opponent.ID, league.ID);

            await testObj.Leave(member.ID);

            Assert.IsEmpty(dbContext.Sets.ToArray());
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
            dbContext.UpdateAndSave(set, () => { set.IsComplete = true; });

            pointService.CalculatePointDeltas(0, 0, false).ReturnsForAnyArgs((1, -1));

            await testObj.UpdateStandings(league.ID);

            Assert.AreNotEqual(originalPoints, player1.Points);
        }

        [Test]
        public async Task UpdateStandings_HasNewcomers_NewcomersSkippedWhenRanking()
        {
            var league = CreateLeague(4);

            dbContext.UpdateRange(league.Members);
            for(var i = 0; i < league.Members.Count; i++)
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

        [TestCase(1, 1, 2, 2, RankTrends.Down)]
        [TestCase(2, 1, 1, 2, RankTrends.None)]
        [TestCase(2, 2, 1, 1, RankTrends.Up)]
        public async Task UpdateStandings_Valid_SetRankTrends(int points, int rank, int otherPoints, int otherRank, RankTrends trend)
        {
            var league = CreateLeague(2);
            league.SetsTillRank = 0;
            var member = league.Members[0];
            member.Rank = rank;
            member.Points = points;
            member.IsNewcomer = false;
            league.Members[1].Rank = otherRank;
            league.Members[1].Points = otherPoints;
            league.Members[1].IsNewcomer = false;

            await testObj.UpdateStandings(league.ID);

            Assert.AreEqual(trend, member.RankTrend);
        }

        [Test]
        public async Task UpdateStandings_WasNewcomer_RankIsAlwaysUp()
        {
            var league = CreateLeague(1);
            league.SetsTillRank = 1;
            var member = league.Members[0];
            member.Rank = 0;
            member.Points = 10;
            member.IsNewcomer = true;
            member.SetCount = 1;

            await testObj.UpdateStandings(league.ID);

            Assert.AreEqual(RankTrends.Up, member.RankTrend);
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

            dbContext.AddNew<RankSnapshot>(rs =>
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

        [Test]
        public void GetUsersRecentCharacters_NoUser_NotFoundException()
        {
            Assert.ThrowsAsync<NotFoundException>(() => testObj.GetUsersRecentCharactersAsync(-1, 3));
        }

        [TestCase(-1)]
        [TestCase(0)]
        public void GetUsersRecentCharacters_CountTooSmall_BadRequestException(int characterCount)
        {
            var league = LeagueUtility.CreateLeague(dbContext);
            var leagueUser = LeagueUtility.AddUsersToLeague(league, 1, dbContext)[0];

            Assert.ThrowsAsync<BadRequestException>(() => testObj.GetUsersRecentCharactersAsync(leagueUser.ID, characterCount));
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public async Task GetUsersRecentCharacters_HasSets_ReturnsCharacters(int characterCount)
        {
            var league = LeagueUtility.CreateLeague(dbContext);
            var members = LeagueUtility.AddUsersToLeague(league, 2, dbContext);
            var leagueUser = members[0];

            var characters = GameUtility.Create(dbContext, characterCount + 1, 0).Characters;
            var set = SetUtility.Create(dbContext, leagueUser.ID, members[1].ID, league.ID);
            var matches = SetUtility.AddMatches(dbContext, set, 2);

            for(int i = 0; i < characterCount + 1; i++)
            {
                dbContext.AddAndSave(new MatchCharacter(matches[0].ID, characters[i].ID, leagueUser.ID, DateTime.MinValue));
            }
            
            for(int i = 0; i < characterCount; i++)
            {
                dbContext.AddAndSave(new MatchCharacter(matches[1].ID, characters[i + 1].ID, leagueUser.ID, DateTime.MinValue));
            }

            var result = await testObj.GetUsersRecentCharactersAsync(leagueUser.ID, characterCount);

            Assert.AreEqual(characterCount, result.Count);
            for(var i = 0; i < result.Count; i++)
            {
                Assert.AreEqual(characters[i + 1].ID, result[i].ID);
            }
        }

        [Test]
        public async Task GetUsersRecentCharacters_NotEnoughMatches_ReturnsAsManyAsPossible()
        {
            const int requestCount = 5;
            const int matchCharacterCount = 3;

            var league = LeagueUtility.CreateLeague(dbContext);
            var members = LeagueUtility.AddUsersToLeague(league, 2, dbContext);
            var leagueUser = members[0];

            var characters = GameUtility.Create(dbContext, matchCharacterCount, 0).Characters;
            var set = SetUtility.Create(dbContext, leagueUser.ID, members[1].ID, league.ID);
            var match = SetUtility.AddMatches(dbContext, set, 1)[0];

            for(int i = 0; i < matchCharacterCount; i++)
            {
                dbContext.AddAndSave(new MatchCharacter(match.ID, characters[i].ID, leagueUser.ID, DateTime.MinValue));
            }

            var result = await testObj.GetUsersRecentCharactersAsync(leagueUser.ID, requestCount);

            Assert.AreEqual(matchCharacterCount, result.Count);
        }

        [Test]
        public async Task GetUsersRecentCharacters_HasCharacters_ReturnsNoDuplicates()
        {
            const int requestCount = 5;
            const int matchCharacterCount = 3;

            var league = LeagueUtility.CreateLeague(dbContext);
            var members = LeagueUtility.AddUsersToLeague(league, 2, dbContext);
            var leagueUser = members[0];

            var characters = GameUtility.Create(dbContext, matchCharacterCount, 0).Characters;
            var set = SetUtility.Create(dbContext, leagueUser.ID, members[1].ID, league.ID);
            var match = SetUtility.AddMatches(dbContext, set, 1)[0];

            for(int i = 0; i < matchCharacterCount; i++)
            {
                dbContext.AddAndSave(new MatchCharacter(match.ID, characters[i].ID, leagueUser.ID, DateTime.MinValue));
            }

            var result = await testObj.GetUsersRecentCharactersAsync(leagueUser.ID, requestCount);

            Assert.AreEqual(matchCharacterCount, result.DistinctBy(c => c.ID).Count());
        }

        #region Helpers
        private Season CreateSeason(int memberCount)
        {
            var league = CreateLeague(memberCount);
            for(var i = 0; i < league.Members.Count; i++)
            {
                var member = league.Members[i];
                member.Points = League.StartingPoints - i;
                member.Rank = i + 1;
            }

            var season = dbContext.AddAndSave(new Season(league.ID, 0, DateTime.MinValue, DateTime.MaxValue));
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
            for(var i = 1; i < league.Members.Count; i++)
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
            var oldLeagueUser = new LeagueUser(league.ID, user.Id) {HasLeft = true};
            dbContext.LeagueUsers.Add(oldLeagueUser);
            dbContext.SaveChanges();
            return oldLeagueUser;
        }
        #endregion
    }
}