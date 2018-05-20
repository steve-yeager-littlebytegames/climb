﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Climb.Data;
using Climb.Exceptions;
using Climb.Requests.Sets;
using Climb.Services.ModelServices;
using Climb.Test.Utilities;
using NUnit.Framework;

namespace Climb.Test.Services.ModelServices
{
    [TestFixture]
    public class SetServiceTest
    {
        private SetService testObj;
        private ApplicationDbContext dbContext;

        [SetUp]
        public void SetUp()
        {
            dbContext = DbContextUtility.CreateMockDb();
            testObj = new SetService(dbContext);
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

        // TODO: Tied scores.

        // TODO: Not matching character counts.

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
    }
}