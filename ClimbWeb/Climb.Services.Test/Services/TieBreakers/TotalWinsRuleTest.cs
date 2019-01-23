using System.Collections.Generic;
using NUnit.Framework;

namespace Climb.Core.TieBreakers.Test
{
    [TestFixture]
    public class TotalWinsRuleTest
    {
        private TotalWinsRule testObj;

        [SetUp]
        public void SetUp()
        {
            testObj = new TotalWinsRule();
        }

        [Test]
        public void GetUserScore_ReturnWinCounts()
        {
            Dictionary<IParticipant, ParticipantRecord> participants = ParticipantsBuilder.Create()
                .Add().Edit(r => AddWins(r, 3))
                .Add().Edit(r => AddWins(r, 5))
                .Add().Edit(r => AddWins(r, 4));

            var results = testObj.Evaluate(participants);

            Assert.AreEqual(3, results[0].score);
            Assert.AreEqual(5, results[1].score);
            Assert.AreEqual(4, results[2].score);
        }

        private static void AddWins(ParticipantRecord record, int winCount)
        {
            var wins = new List<int>();
            for(var i = 0; i < winCount; i++)
            {
                wins.Add(0);
            }

            record.AddWins(wins);
        }
    }
}