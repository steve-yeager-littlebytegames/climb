using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Climb.Data;
using Climb.Exceptions;
using Climb.Models;
using Climb.Requests.Sets;
using Climb.Services.ModelServices;
using Climb.Test.Utilities;
using NSubstitute;
using NUnit.Framework;

namespace Climb.Test.Services.ModelServices
{
    // TODO: Tied scores.
    // TODO: Not matching character counts.
    // TODO: Already have an open set request.
    // TODO: RespondToSetRequestAsync: Request approved and date set.
    [TestFixture]
    public class SetServiceTest
    {
        private SetService testObj;
        private ApplicationDbContext dbContext;
        private ISeasonService seasonService;

        [SetUp]
        public void SetUp()
        {
            dbContext = DbContextUtility.CreateMockDb();
            seasonService = Substitute.For<ISeasonService>();

            testObj = new SetService(dbContext, seasonService);
        }

        [Test]
        public async Task Update_FirstMatches_CreatesMatchesAndMatchCharacters()
        {
            var set = SetUtility.Create(dbContext);

            var matchForms = CreateMatchForms(3);
            await testObj.Update(set.ID, matchForms);

            Assert.AreEqual(3, set.Matches.Count);
        }

        [Test]
        public async Task Update_NewMatches_ReplacesOldMatchesAndMatchCharacters()
        {
            var set = SetUtility.Create(dbContext);
            var matchForms = CreateMatchForms(3);
            await testObj.Update(set.ID, matchForms);

            await testObj.Update(set.ID, matchForms);

            Assert.AreEqual(3, set.Matches.Count);
        }

        [Test]
        public async Task Update_RemoveMatches_DeletesOldMatchesAndMatchCharacters()
        {
            var set = SetUtility.Create(dbContext);
            var matchForms = CreateMatchForms(3);
            await testObj.Update(set.ID, matchForms);

            await testObj.Update(set.ID, new MatchForm[0]);

            Assert.AreEqual(0, set.Matches.Count);
        }

        [Test]
        public void Update_NoSet_NotFoundException()
        {
            var matchForms = CreateMatchForms(3);
            Assert.ThrowsAsync<NotFoundException>(() => testObj.Update(0, matchForms));
        }

        [Test]
        public async Task Update_HasWinner_UpdateScore()
        {
            var set = SetUtility.Create(dbContext);

            var matchForms = CreateMatchFormsWithScores(1, 2, 1);
            matchForms.AddRange(CreateMatchFormsWithScores(2, 0, 2));

            await testObj.Update(set.ID, matchForms);

            Assert.AreEqual(1, set.Player1Score);
            Assert.AreEqual(2, set.Player2Score);
        }

        [Test]
        public async Task Update_IsSeasonSet_UpdatesSeasonPoints()
        {
            var set = SetUtility.Create(dbContext);

            var matchForms = CreateMatchForms(3);
            await testObj.Update(set.ID, matchForms);

#pragma warning disable 4014
            seasonService.Received(1).UpdateStandings(set.ID);
#pragma warning restore 4014
        }

        [Test]
        public async Task Update_IsNotSeasonSet_DoesNotUpdateSeasonPoints()
        {
            var set = SetUtility.Create(dbContext);
            set.SeasonID = null;

            var matchForms = CreateMatchForms(3);
            await testObj.Update(set.ID, matchForms);

#pragma warning disable 4014
            seasonService.DidNotReceive().UpdateStandings(set.ID);
#pragma warning restore 4014
        }

        [Test]
        public async Task Update_NewSet_LeagueUserSetCountIncremented()
        {
            var set = SetUtility.Create(dbContext);
            var matchForms = CreateMatchForms(3);

            await testObj.Update(set.ID, matchForms);
            await testObj.Update(set.ID, matchForms);

            Assert.AreEqual(1, set.Player1.SetCount);
            Assert.AreEqual(1, set.Player2.SetCount);
        }
        
        [Test]
        public async Task Update_OldSet_LeagueUserSetCountNotChanged()
        {
            var set = SetUtility.Create(dbContext);
            var matchForms = CreateMatchForms(3);

            await testObj.Update(set.ID, matchForms);

            Assert.AreEqual(1, set.Player1.SetCount);
            Assert.AreEqual(1, set.Player2.SetCount);
        }

        [Test]
        public async Task Update_IsCompleteIsTrue()
        {
            var set = SetUtility.Create(dbContext);

            var matchForms = CreateMatchForms(3);
            await testObj.Update(set.ID, matchForms);

            Assert.IsTrue(set.IsComplete);
        }

        [Test]
        public void RequestSet_NoRequester_NotFoundException()
        {
            var league = LeagueUtility.CreateLeague(dbContext);
            var challenged = LeagueUtility.AddUsersToLeague(league, 1, dbContext)[0];

            Assert.ThrowsAsync<NotFoundException>(() => testObj.RequestSetAsync(0, challenged.ID, null));
        }

        [Test]
        public void RequestSet_NoChallenged_NotFoundException()
        {
            var league = LeagueUtility.CreateLeague(dbContext);
            var requester = LeagueUtility.AddUsersToLeague(league, 1, dbContext)[0];

            Assert.ThrowsAsync<NotFoundException>(() => testObj.RequestSetAsync(requester.ID, 0, null));
        }

        [Test]
        public async Task RequestSet_Valid_DateSet()
        {
            var league = LeagueUtility.CreateLeague(dbContext);
            LeagueUtility.AddUsersToLeague(league, 2, dbContext);
            var requester = league.Members[0];
            var challenged = league.Members[1];

            var request = await testObj.RequestSetAsync(requester.ID, challenged.ID, null);

            Assert.AreEqual(DateTime.Today.ToShortDateString(), request.DateCreated.ToShortDateString());
        }

        [Test]
        public async Task RequestSet_Valid_SetLeague()
        {
            var league = LeagueUtility.CreateLeague(dbContext);
            LeagueUtility.AddUsersToLeague(league, 2, dbContext);
            var requester = league.Members[0];
            var challenged = league.Members[1];

            var request = await testObj.RequestSetAsync(requester.ID, challenged.ID, null);

            Assert.AreEqual(league.ID, request.LeagueID);
        }

        [Test]
        public void RespondToSetRequestAsync_NoRequest_NotFoundException()
        {
            Assert.ThrowsAsync<NotFoundException>(() => testObj.RespondToSetRequestAsync(0, false));
        }

        [Test]
        public void RespondToSetRequest_NoRequest_NotFoundException()
        {
            Assert.ThrowsAsync<NotFoundException>(() => testObj.RespondToSetRequestAsync(0, true));
        }

        [Test]
        public async Task RespondToSetRequest_Approved_CreateSet()
        {
            var setRequest = CreateSetRequest(true);

            setRequest = await testObj.RespondToSetRequestAsync(setRequest.ID, true);

            Assert.IsNotNull(setRequest.SetID);
        }

        [Test]
        public async Task RespondToSetRequest_Declined_NoSet()
        {
            var setRequest = CreateSetRequest(true);

            setRequest = await testObj.RespondToSetRequestAsync(setRequest.ID, false);

            Assert.IsNull(setRequest.SetID);
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task RespondToSetRequest_Responded_IsClosed(bool accepted)
        {
            var setRequest = CreateSetRequest(true);
            Assert.IsTrue(setRequest.IsOpen);

            setRequest = await testObj.RespondToSetRequestAsync(setRequest.ID, accepted);

            Assert.IsFalse(setRequest.IsOpen);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void RespondToSetRequest_NotOpen_BadRequestException(bool accepted)
        {
            var setRequest = CreateSetRequest(false);

            Assert.ThrowsAsync<BadRequestException>(() => testObj.RespondToSetRequestAsync(setRequest.ID, accepted));
        }

        private static List<MatchForm> CreateMatchFormsWithScores(int count, int p1Score, int p2Score)
        {
            var forms = CreateMatchForms(count);
            foreach(var matchForm in forms)
            {
                matchForm.Player1Score = p1Score;
                matchForm.Player2Score = p2Score;
            }

            return forms;
        }

        private static List<MatchForm> CreateMatchForms(int count)
        {
            var matchForms = new List<MatchForm>(count);
            for(var i = 0; i < count; i++)
            {
                matchForms.Add(new MatchForm
                {
                    Player1Score = 1,
                    Player2Score = 2,
                    Player1Characters = new[] {3, 1, 2},
                    Player2Characters = new[] {2, 1, 3},
                    StageID = 1,
                });
            }

            return matchForms;
        }

        private SetRequest CreateSetRequest(bool isOpen)
        {
            var league = LeagueUtility.CreateLeague(dbContext);
            var members = LeagueUtility.AddUsersToLeague(league, 2, dbContext);
            var setRequest = DbContextUtility.AddNew<SetRequest>(dbContext, sr =>
            {
                sr.LeagueID = league.ID;
                sr.RequesterID = members[0].ID;
                sr.ChallengedID = members[1].ID;
                sr.IsOpen = isOpen;
            });
            return setRequest;
        }
    }
}