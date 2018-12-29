using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Climb.Core.TieBreakers;
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
    public class SeasonServiceTest
    {
        private SeasonService testObj;
        private ApplicationDbContext dbContext;
        private IScheduleFactory scheduler;
        private ISeasonPointCalculator pointCalculator;
        private ITieBreaker tieBreaker;
        private IDateService dateService;

        private static DateTime StartDate => DateTime.Now.AddDays(1);
        private static DateTime EndDate => DateTime.Now.AddDays(2);

        [SetUp]
        public void SetUp()
        {
            dbContext = DbContextUtility.CreateMockDb();
            scheduler = Substitute.For<IScheduleFactory>();
            pointCalculator = Substitute.For<ISeasonPointCalculator>();
            var tieBreakerFactory = Substitute.For<ITieBreakerFactory>();
            tieBreaker = Substitute.For<ITieBreaker>();
            dateService = Substitute.For<IDateService>();

            tieBreakerFactory.Create(Arg.Any<DateTime>()).Returns(tieBreaker);

            testObj = new SeasonService(dbContext, scheduler, pointCalculator, tieBreakerFactory, dateService);
        }

        [Test]
        public async Task Create_Valid_NotNull()
        {
            var league = LeagueUtility.CreateLeague(dbContext);

            var season = await testObj.Create(league.ID, StartDate, EndDate);

            Assert.IsNotNull(season);
        }

        [Test]
        public void Create_NoLeague_NotFoundException()
        {
            Assert.ThrowsAsync<NotFoundException>(() => testObj.Create(0, StartDate, EndDate));
        }

        [Test]
        public void Create_StartInPast_BadRequestException()
        {
            dateService.Now.Returns(DateTime.Now);

            Assert.ThrowsAsync<BadRequestException>(() => testObj.Create(0, DateTime.MinValue, EndDate));
        }

        [Test]
        public void Create_EndBeforeStart_BadRequestException()
        {
            Assert.ThrowsAsync<BadRequestException>(() => testObj.Create(0, DateTime.Now.AddDays(2), StartDate));
        }

        [Test]
        public async Task Create_Valid_AddsMembers()
        {
            var league = LeagueUtility.CreateLeague(dbContext);
            LeagueUtility.AddUsersToLeague(league, 1, dbContext);

            var season = await testObj.Create(league.ID, StartDate, EndDate);

            Assert.IsTrue(season.Participants.Count > 0);
        }

        [Test]
        public void GenerateSchedule_NoSeason_NotFoundException()
        {
            Assert.ThrowsAsync<NotFoundException>(() => testObj.GenerateSchedule(0));
        }

        [Test]
        public async Task PlaySets_Valid_PointsAssigned()
        {
            var (winnerPoints, loserPoints) = (2, 1);
            var set = CreateSet(loserPoints, winnerPoints);

            await testObj.PlaySet(set.ID);

            Assert.AreEqual(loserPoints, set.Season.Participants.First(slu => slu.LeagueUserID == set.Player1ID).Points);
            Assert.AreEqual(winnerPoints, set.Season.Participants.First(slu => slu.LeagueUserID == set.Player2ID).Points);
        }

        [Test]
        public async Task PlaySets_Valid_PointsSaved()
        {
            var (winnerPoints, loserPoints) = (2, 1);
            var set = CreateSet(loserPoints, winnerPoints);

            await testObj.PlaySet(set.ID);

            Assert.AreEqual(loserPoints, set.Player1SeasonPoints);
            Assert.AreEqual(winnerPoints, set.Player2SeasonPoints);
        }

        [Test]
        public async Task UpdateRanks_NoTies_RanksUpdated()
        {
            var season = CreateSeason((0, 10, 0), (0, 20, 0));

            await testObj.UpdateRanksAsync(season.ID);

            Assert.AreEqual(2, season.Participants[0].Standing);
            Assert.AreEqual(1, season.Participants[1].Standing);
        }

        [TestCase(0, 0, 0, 0)]
        [TestCase(0, 0, 1, 1)]
        [TestCase(1, 1, 0, 0)]
        [TestCase(1, 0, 0, 1)]
        public async Task UpdateRanks_Ties_NoTiedStandings(params int[] seasonPoints)
        {
            MockTieBreak();

            var season = CreateSeason(seasonPoints.Select(p => (0, p, 0)).ToArray());

            await testObj.UpdateRanksAsync(season.ID);

            season.Participants.Sort();
            for(var i = 0; i < season.Participants.Count; i++)
            {
                Assert.AreEqual(4 - i, season.Participants[i].Standing);
            }
        }

        [Test]
        public async Task UpdateRanks_TieBroken_TieBreakScoresReset()
        {
            var season = CreateSeason((1, 5, 100), (2, 5, 10));

            await testObj.UpdateRanksAsync(season.ID);

            Assert.AreEqual(0, season.Participants[0].TieBreakerPoints);
            Assert.AreEqual(0, season.Participants[1].TieBreakerPoints);
        }

        [Test]
        public async Task UpdateRanks_Ties_TiedUsersHaveTieBreakPoints()
        {
            var season = CreateSeason((0, 5, 10), (0, 1, 10), (0, 5, 10));
            var p1 = season.Participants[0];
            var p2 = season.Participants[1];
            var p3 = season.Participants[2];
            MockTieBreak();

            await testObj.UpdateRanksAsync(season.ID);

            Assert.Greater(p1.TieBreakerPoints, 0);
            Assert.AreEqual(0, p2.TieBreakerPoints);
            Assert.Greater(p3.TieBreakerPoints, 0);
        }

        [Test]
        public async Task UpdateRanks_UsersHaveLeft_RanksIgnoreUsersThatLeft()
        {
            var season = CreateSeason((0, 30, 0), (0, 20, 0), (0, 10, 0));
            var player1 = season.Participants[0];
            var player2 = season.Participants[1];
            var player3 = season.Participants[2];

            DbContextUtility.UpdateAndSave(dbContext, player2, slu => slu.HasLeft = true);

            await testObj.UpdateRanksAsync(season.ID);

            Assert.AreEqual(1, player1.Standing);
            Assert.AreEqual(0, player2.Standing);
            Assert.AreEqual(2, player3.Standing);
        }

        [Test]
        public async Task End_HasNonCompletedSets_SetsDeleted()
        {
            const int setCount = 4;
            const int completedSets = 2;
            var (season, participants) = SeasonUtility.CreateSeason(dbContext, 2, s => s.IsActive = true);
            var sets = new List<Set>();

            for(var i = 0; i < setCount; i++)
            {
                sets.Add(SetUtility.Create(dbContext, participants[0].ID, participants[1].ID, season.LeagueID, season));
            }

            for(var i = 0; i < completedSets; i++)
            {
                var setIndex = i;
                DbContextUtility.UpdateAndSave(dbContext, sets[setIndex], () => sets[setIndex].IsComplete = true);
            }

            season = await testObj.End(season.ID);

            Assert.AreEqual(setCount - completedSets, season.Sets.Count);
        }

        [Test]
        public async Task End_HasCompletedNonLockedSets_SetsLocked()
        {
            const int setCount = 4;
            const int lockedSets = 2;
            var (season, participants) = SeasonUtility.CreateSeason(dbContext, 2, s => s.IsActive = true);
            var sets = new List<Set>();

            for(var i = 0; i < setCount; i++)
            {
                sets.Add(SetUtility.Create(dbContext, participants[0].ID, participants[1].ID, season.LeagueID, season));
                var setIndex = i;
                DbContextUtility.UpdateAndSave(dbContext, sets[setIndex], () => sets[setIndex].IsComplete = true);
            }

            for(var i = 0; i < lockedSets; i++)
            {
                var setIndex = i;
                DbContextUtility.UpdateAndSave(dbContext, sets[setIndex], () => sets[setIndex].IsLocked = true);
            }

            season = await testObj.End(season.ID);

            Assert.AreEqual(setCount, season.Sets.Count(s => s.IsLocked));
        }

        [Test]
        public void End_NoSeason_NotFoundException()
        {
            Assert.ThrowsAsync<NotFoundException>(() => testObj.End(0));
        }

        [Test]
        public void End_NotStarted_BadRequestException()
        {
            var (season, _) = SeasonUtility.CreateSeason(dbContext, 2);

            Assert.ThrowsAsync<BadRequestException>(() => testObj.End(season.ID));
        }

        [Test]
        public void End_Completed_BadRequestException()
        {
            var (season, _) = SeasonUtility.CreateSeason(dbContext, 2, s => s.IsComplete = s.IsActive = true);

            Assert.ThrowsAsync<BadRequestException>(() => testObj.End(season.ID));
        }

        [Test]
        public async Task End_Valid_MarkComplete()
        {
            var (season, _) = SeasonUtility.CreateSeason(dbContext, 2, s => s.IsActive = true);

            season = await testObj.End(season.ID);

            Assert.IsTrue(season.IsComplete);
        }

        [Test]
        public async Task End_Valid_MarkNotActive()
        {
            var (season, _) = SeasonUtility.CreateSeason(dbContext, 2, s => s.IsActive = true);

            season = await testObj.End(season.ID);

            Assert.IsFalse(season.IsActive);
        }

        [Test]
        public async Task End_Valid_RemoveLeagueActiveSeason()
        {
            var (season, _) = SeasonUtility.CreateSeason(dbContext, 2, s => s.IsActive = true);
            int seasonID = season.ID;
            DbContextUtility.UpdateAndSave(dbContext, season.League, l => l.ActiveSeasonID = seasonID);

            Assert.IsNotNull(season.League.ActiveSeasonID);

            season = await testObj.End(seasonID);

            Assert.IsNull(season.League.ActiveSeasonID);
        }

        [Test]
        public async Task Leave_IsParticipant_MarkedAsLeft()
        {
            var season = CreateSeason((0, 0, 0));
            var participant = season.Participants[0];

            await testObj.LeaveAsync(participant.ID);

            Assert.IsTrue(participant.HasLeft);
        }

        [Test]
        public async Task Leave_HasLeft_NoException()
        {
            var season = CreateSeason((0, 0, 0));
            var participant = season.Participants[0];
            DbContextUtility.UpdateAndSave(dbContext, participant, () => participant.HasLeft = true);

            await testObj.LeaveAsync(participant.ID);

            Assert.Pass();
        }

        [Test]
        public void Leave_NotFound_NotFoundException()
        {
            CreateSeason((0, 0, 0));

            Assert.ThrowsAsync<NotFoundException>(() => testObj.LeaveAsync(-1));
        }

        [Test]
        public async Task Leave_IsParticipant_ForfeitSets()
        {
            var season = CreateSeason((0, 0, 0), (0, 0, 0));
            var participant = season.Participants[0];
            CreateSets(season, participant);

            await testObj.LeaveAsync(participant.ID);

            Assert.IsNotEmpty(participant.P1Sets, "P1 Sets");
            Assert.IsNotEmpty(participant.P2Sets, "P2 Sets");
            Assert.IsTrue(participant.P1Sets.All(s => s.IsForfeit), "P1 Sets");
            Assert.IsTrue(participant.P2Sets.All(s => s.IsForfeit), "P2 Sets");
        }

        [Test]
        public async Task Leave_HasPoints_StatsZeroed()
        {
            var season = CreateSeason((2, 10, 15));
            var participant = season.Participants[0];

            await testObj.LeaveAsync(participant.ID);

            Assert.AreEqual(0, participant.Standing);
            Assert.AreEqual(0, participant.Points);
            Assert.AreEqual(0, participant.TieBreakerPoints);
        }

        [Test]
        public void Leave_SeasonComplete_BadRequestException()
        {
            var season = CreateSeason((0, 0, 0));
            DbContextUtility.UpdateAndSave(dbContext, season, () => season.IsComplete = true);
            var participant = season.Participants[0];

            Assert.ThrowsAsync<BadRequestException>(() => testObj.LeaveAsync(participant.ID));
        }

        [Test]
        public async Task Leave_SeasonNotStarted_StandingsNotUpdated()
        {
            var season = CreateSeason((0, 0, 0), (0, 0, 0), (0, 0, 0));

            await testObj.LeaveAsync(season.Participants[0].ID);

            Assert.IsTrue(season.Participants.All(slu => slu.Standing == 0));
        }

        [Test]
        public async Task Leave_HasPlayedSets_CompletedSetsNotForfeited()
        {
            var season = CreateSeason((0, 0, 0), (0, 0, 0));
            var participant = season.Participants[0];
            var set = SeasonUtility.CreateSets(dbContext, season)[0];
            DbContextUtility.UpdateAndSave(dbContext, set, s => s.IsComplete = true);

            await testObj.LeaveAsync(participant.ID);

            Assert.IsNotEmpty(participant.P1Sets, "P1 Sets");
            Assert.IsFalse(participant.P1Sets.All(s => s.IsForfeit), "P1 Sets");
        }

        [Test]
        public async Task Join_SeasonNotStarted_CreateParticipant()
        {
            var season = CreateSeason((0, 0, 0));
            var leagueUser = LeagueUtility.AddUsersToLeague(season.League, 1, dbContext)[0];

            await testObj.JoinAsync(season.ID, leagueUser.UserID);

            Assert.AreEqual(2, season.Participants.Count);
        }

        [Test]
        public void Join_NoSeason_NotFoundException()
        {
            var user = DbContextUtility.AddNew<ApplicationUser>(dbContext);

            Assert.ThrowsAsync<NotFoundException>(() => testObj.JoinAsync(-1, user.Id));
        }

        [Test]
        public void Join_NoUser_NotFoundException()
        {
            var season = CreateSeason();

            Assert.ThrowsAsync<NotFoundException>(() => testObj.JoinAsync(season.ID, ""));
        }

        [Test]
        public async Task Join_AlreadyJoined_NoUserCreated()
        {
            var season = CreateSeason((0, 0, 0));
            var user = season.Participants[0].User;

            await testObj.JoinAsync(season.ID, user.Id);

            Assert.AreEqual(1, season.Participants.Count);
        }

        [Test]
        public async Task Join_HasLeftSeasonNotStarted_ReactivateParticipant()
        {
            var season = CreateSeason((0, 0, 0));
            var participant = season.Participants[0];
            DbContextUtility.UpdateAndSave(dbContext, participant, () => participant.HasLeft = true);

            await testObj.JoinAsync(season.ID, participant.UserID);

            Assert.IsFalse(participant.HasLeft);
            Assert.AreEqual(1, season.Participants.Count);
        }

        [Test]
        public async Task Join_SeasonStartedNewJoin_CreateSets()
        {
            scheduler.GenerateSchedule(default, default, default).ReturnsForAnyArgs(new List<Set>());
            var season = CreateSeason();
            DbContextUtility.UpdateAndSave(dbContext, season, s => s.IsActive = true);
            var leagueUser = LeagueUtility.AddUsersToLeague(season.League, 1, dbContext)[0];

            await testObj.JoinAsync(season.ID, leagueUser.UserID);

            scheduler.Received(1).GenerateSchedule(season.StartDate, season.EndDate, season.Participants);
            scheduler.Received(1).Reschedule(default, season.EndDate, Arg.Any<IReadOnlyList<Set>>(), Arg.Any<IReadOnlyList<SeasonLeagueUser>>());
        }

        [Test]
        public async Task Join_SeasonStartedRejoin_CreateSets()
        {
            scheduler.GenerateSchedule(default, default, default).ReturnsForAnyArgs(new List<Set>());
            var season = CreateSeason((0, 0, 0));
            DbContextUtility.UpdateAndSave(dbContext, season, s => s.IsActive = true);
            var participant = season.Participants[0];
            DbContextUtility.UpdateAndSave(dbContext, participant, p => p.HasLeft = true);

            await testObj.JoinAsync(season.ID, participant.UserID);

            scheduler.Received(1).GenerateSchedule(season.StartDate, season.EndDate, season.Participants);
            scheduler.Received(1).Reschedule(default, season.EndDate, Arg.Any<IReadOnlyList<Set>>(), Arg.Any<IReadOnlyList<SeasonLeagueUser>>());
        }

        #region Helpers
        private Season CreateSeason(params (int standing, int points, int tieBreak)[] participants)
        {
            var season = SeasonUtility.CreateSeason(dbContext, participants.Length).season;
            for(var i = 0; i < participants.Length; i++)
            {
                season.Participants[i].Standing = participants[i].standing;
                season.Participants[i].Points = participants[i].points;
                season.Participants[i].TieBreakerPoints = participants[i].tieBreak;
            }

            return season;
        }

        private Set CreateSet(int p1Score, int p2Score)
        {
            var winnerScore = p1Score > p2Score ? p1Score : p2Score;
            var loserScore = p1Score > p2Score ? p2Score : p1Score;

            pointCalculator.CalculatePointDeltas(null, null).ReturnsForAnyArgs((winnerScore, loserScore));
            var season = SeasonUtility.CreateSeason(dbContext, 2).season;
            var set = SeasonUtility.CreateSets(dbContext, season)[0];
            set.Player1Score = p1Score;
            set.Player2Score = p2Score;

            return set;
        }

        private void MockTieBreak()
        {
            tieBreaker.WhenForAnyArgs(tb => tb.Break(null)).Do(info =>
            {
                var arg = info.Arg<Dictionary<IParticipant, ParticipantRecord>>();
                var points = 1;
                foreach(var entry in arg)
                {
                    entry.Key.TieBreakerPoints = points;
                    ++points;
                }
            });
        }

        private void CreateSets(Season season, SeasonLeagueUser participant)
        {
            var opponent = season.Participants[1];
            SetUtility.Create(dbContext, participant, opponent, season.LeagueID);
            SetUtility.Create(dbContext, opponent, participant, season.LeagueID);
        } 
        #endregion
    }
}