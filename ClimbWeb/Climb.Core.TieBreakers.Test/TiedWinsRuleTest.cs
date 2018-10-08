using System.Collections.Generic;
using NUnit.Framework;

namespace Climb.Core.TieBreakers.Test
{
    [TestFixture]
    public class TiedWinsRuleTest
    {
        private TiedWinsRule testObj;

        [SetUp]
        public void SetUp()
        {
            testObj = new TiedWinsRule();
        }

        [Test]
        public void Evaluate_ReturnTiedWins()
        {
            Dictionary<IParticipant, ParticipantRecord> participants = ParticipantsBuilder.Create()
                .Add(1).Edit(r => AddWins(r))
                .Add(2).Edit(r => AddWins(r, 1, 3))
                .Add(3).Edit(r => AddWins(r, 1));

            var results = testObj.Evaluate(participants);

            Assert.AreEqual(0, results[0].score);
            Assert.AreEqual(2, results[1].score);
            Assert.AreEqual(1, results[2].score);
        }

        private static void AddWins(ParticipantRecord record, params int[] beatenOpponents)
        {
            record.AddWins(beatenOpponents);
        }
    }
}