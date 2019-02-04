﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Climb.Data;
using Climb.Exceptions;
using Climb.Models;
using Climb.Services;
using Climb.Test.Utilities;
using NSubstitute;
using NUnit.Framework;

namespace Climb.Test.Services.ModelServices
{
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
        public void Create_SeasonDoesntMatchLeague_BadRequestException()
        {
            var league = dbContext.CreateLeague();
            var season = dbContext.CreateSeason(0).season;

            Assert.ThrowsAsync<BadRequestException>(() => testObj.Create(league.ID, season.ID, "TestName"));
        }

        [Test]
        public void Join_NoTournament_NotFoundException()
        {
            Assert.ThrowsAsync<NotFoundException>(() => testObj.Join(-1, ""));
        }

        [Test]
        public void Join_NoLeagueUser_NotFoundException()
        {
            var tournament = dbContext.CreateTournament(DateTime.MinValue);

            Assert.ThrowsAsync<NotFoundException>(() => testObj.Join(tournament.ID, ""));
        }

        [Test]
        public async Task Join_NewUser_AddTournamentUser()
        {
            var tournament = dbContext.CreateTournament(DateTime.MinValue);
            var member = dbContext.AddUsersToLeague(tournament.League, 1)[0];

            var competitor = await testObj.Join(tournament.ID, member.UserID);

            Assert.IsNotNull(competitor);
        }

        [Test]
        public async Task Join_NewUser_BottomSeed()
        {
            var tournament = dbContext.CreateTournament(DateTime.MinValue);
            dbContext.AddCompetitors(tournament, 4);
            var member = dbContext.AddUsersToLeague(tournament.League, 1)[0];

            var competitor = await testObj.Join(tournament.ID, member.UserID);

            Assert.AreEqual(tournament.TournamentUsers.Count, competitor.Seed);
        }

        [Test]
        public async Task Join_NewUser_LinkSeasonUser()
        {
            var season = dbContext.CreateSeason(1).season;
            var tournament = dbContext.CreateTournament(DateTime.MinValue, season.League);
            var participant = season.Participants[0];

            var competitor = await testObj.Join(tournament.ID, participant.UserID);

            Assert.AreEqual(participant.ID, competitor.SeasonLeagueUserID);
        }

        [Test]
        public async Task Join_AlreadyJoined_NoNewUser()
        {
            var tournament = dbContext.CreateTournament(DateTime.MinValue);
            var member = dbContext.AddUsersToLeague(tournament.League, 1)[0];

            await testObj.Join(tournament.ID, member.UserID);
            var competitor = await testObj.Join(tournament.ID, member.UserID);

            Assert.IsNotNull(competitor);
            Assert.AreEqual(1, tournament.TournamentUsers.Count);
        }

        [Test]
        public void Join_HasStarted_BadRequestException()
        {
            var (member, tournament) = CreateTournament(Tournament.States.Active);

            Assert.ThrowsAsync<BadRequestException>(() => testObj.Join(tournament.ID, member.UserID));
        }

        [Test]
        public void Join_HasFinished_BadRequestException()
        {
            var (member, tournament) = CreateTournament(Tournament.States.Complete);

            Assert.ThrowsAsync<BadRequestException>(() => testObj.Join(tournament.ID, member.UserID));
        }

        [Test]
        public void Leave_NoCompetitor_NotFoundException()
        {
            Assert.ThrowsAsync<NotFoundException>(() => testObj.Leave(-1));
        }

        [Test]
        public void Leave_HasStarted_BadRequestException()
        {
            var competitor = CreateTournamentWithCompetitor(Tournament.States.Active);

            Assert.ThrowsAsync<BadRequestException>(() => testObj.Leave(competitor.ID));
        }

        [Test]
        public void Leave_HasFinished_BadRequestException()
        {
            var competitor = CreateTournamentWithCompetitor(Tournament.States.Complete);

            Assert.ThrowsAsync<BadRequestException>(() => testObj.Leave(competitor.ID));
        }

        [Test]
        public async Task Leave_UpdateSeeds_SeedsAligned()
        {
            var tournament = dbContext.CreateTournament(DateTime.MinValue);
            var competitors = dbContext.AddCompetitors(tournament, 4);

            await testObj.Leave(competitors[1].ID);

            var tournamentUsers = tournament.TournamentUsers;
            tournamentUsers.Sort((x, y) => x.Seed.CompareTo(y.Seed));

            Assert.AreEqual(competitors.Length - 1, tournamentUsers.Count);
            for(int i = 0; i < tournamentUsers.Count; i++)
            {
                Assert.AreEqual(i + 1, tournamentUsers[i].Seed);
            }
        }

        #region Helper
        private (LeagueUser, Tournament) CreateTournament(Tournament.States state)
        {
            var tournament = dbContext.CreateTournament(DateTime.MinValue);
            dbContext.UpdateAndSave(tournament, t => t.State = state);
            var member = dbContext.AddUsersToLeague(tournament.League, 1)[0];
            return (member, tournament);
        }

        private TournamentUser CreateTournamentWithCompetitor(Tournament.States state)
        {
            var tournament = dbContext.CreateTournament(DateTime.MinValue);
            var competitor = dbContext.AddCompetitors(tournament, 1)[0];
            dbContext.UpdateAndSave(tournament, t => t.State = state);
            return competitor;
        }
        #endregion
    }
}