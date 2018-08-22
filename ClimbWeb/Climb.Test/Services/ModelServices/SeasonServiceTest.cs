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

            tieBreakerFactory.Create().Returns(tieBreaker);

            testObj = new SeasonService(dbContext, scheduler, pointCalculator, tieBreakerFactory);
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
        public async Task UpdateStandings_Valid_PointsAssigned()
        {
            var (winnerPoints, loserPoints) = (2, 1);
            var set = CreateSet(loserPoints, winnerPoints);

            await testObj.UpdateStandings(set.ID);

            Assert.AreEqual(loserPoints, set.Season.Participants.First(slu => slu.LeagueUserID == set.Player1ID).Points);
            Assert.AreEqual(winnerPoints, set.Season.Participants.First(slu => slu.LeagueUserID == set.Player2ID).Points);
        }

        [Test]
        public async Task UpdateStandings_Valid_PointsSaved()
        {
            var (winnerPoints, loserPoints) = (2, 1);
            var set = CreateSet(loserPoints, winnerPoints);

            await testObj.UpdateStandings(set.ID);

            Assert.AreEqual(loserPoints, set.Player1SeasonPoints);
            Assert.AreEqual(winnerPoints, set.Player2SeasonPoints);
        }

        [Test]
        public async Task UpdateStandings_NoTies_RanksUpdated()
        {
            var (winnerPoints, loserPoints) = (2, 1);
            var set = CreateSet(loserPoints, winnerPoints);

            await testObj.UpdateStandings(set.ID);

            Assert.AreEqual(1, set.SeasonPlayer2.Standing);
            Assert.AreEqual(2, set.SeasonPlayer1.Standing);
        }

        [TestCase(0, 0, 0, 0)]
        [TestCase(0, 0, 1, 1)]
        [TestCase(1, 1, 0, 0)]
        [TestCase(1, 0, 0, 1)]
        public async Task UpdateStandings_Ties_NoTiedStandings(params int[] seasonPoints)
        {
            MockTieBreak(arg =>
            {
                var points = 1;
                foreach(var entry in arg)
                {
                    entry.Key.TieBreakerPoints = points;
                    ++points;
                }
            });

            var season = SeasonUtility.CreateSeason(dbContext, 4).season;
            dbContext.UpdateRange(season.Participants);
            for(var i = 0; i < seasonPoints.Length; i++)
            {
                season.Participants[i].Points = seasonPoints[i];
            }

            dbContext.SaveChanges();

            var set = SetUtility.Create(dbContext, season.Participants[0], season.Participants[1], season.LeagueID);

            await testObj.UpdateStandings(set.ID);

            season.Participants.Sort();
            for(var i = 0; i < season.Participants.Count; i++)
            {
                Assert.AreEqual(4 - i, season.Participants[i].Standing);
            }
        }

        [Test]
        public async Task UpdateStandings_TieBrokenWithWin_TieBreakScoresReset()
        {
            var season = CreateSeason((1, 5, 100), (2, 5, 10));
            var player1 = season.Participants[0];
            var player2 = season.Participants[1];

            var set = SetUtility.Create(dbContext, player1, player2, season.LeagueID);

            await testObj.UpdateStandings(set.ID);

            Assert.AreEqual(0, player1.TieBreakerPoints);
            Assert.AreEqual(0, player2.TieBreakerPoints);
        }

        [Test]
        public async Task UpdateStandings_3WayTieBrokenWithWin_2TiedUsersHaveCorrectTieBreakScores()
        {
            var season = CreateSeason((1, 5, 1000), (2, 5, 100), (3, 5, 10), (4, 2, 0));
            var player1 = season.Participants[0];
            var player2 = season.Participants[1];
            var player3 = season.Participants[2];
            var player4 = season.Participants[3];

            MockTieBreak(arg =>
            {
                arg.First(p => p.Key.ID == player1.ID).Key.TieBreakerPoints = 100;
                arg.First(p => p.Key.ID == player3.ID).Key.TieBreakerPoints = 10;
            });

            pointCalculator.CalculatePointDeltas(player2, player4).Returns((2, 1));

            var set = SetUtility.Create(dbContext, player2, player4, season.LeagueID);
            DbContextUtility.UpdateAndSave(dbContext, set, () =>
            {
                set.Player1Score = 1;
                set.Player2Score = 0;
            });

            await testObj.UpdateStandings(set.ID);

            Assert.Greater(player1.TieBreakerPoints, 0);
            Assert.Greater(player3.TieBreakerPoints, 0);

            Assert.AreEqual(0, player2.TieBreakerPoints);
        }

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

        private void MockTieBreak(Action<Dictionary<IParticipant, ParticipantRecord>> onBreak)
        {
            tieBreaker.WhenForAnyArgs(tb => tb.Break(null)).Do(info =>
            {
                var arg = info.Arg<Dictionary<IParticipant, ParticipantRecord>>();
                onBreak(arg);
            });
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
    }
}