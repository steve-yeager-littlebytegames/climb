using System.Collections.Generic;
using NUnit.Framework;

namespace Climb.Core.TieBreakers.Test
{
    [TestFixture]
    public class LeaguePointsRuleTest
    {
        private LeaguePointsRule testObj;

        [SetUp]
        public void SetUp()
        {
            testObj = new LeaguePointsRule();
        }

        [Test]
        public void Evaluate_ReturnLeaguePointTotals()
        {
            Dictionary<IParticipant, ParticipantRecord> participants = ParticipantsBuilder.Create()
                .Add(leaguePoints: 10)
                .Add(leaguePoints: 20)
                .Add(leaguePoints: 30);

            var results = testObj.Evaluate(participants);

            Assert.AreEqual(10, results[0].score);
            Assert.AreEqual(20, results[1].score);
            Assert.AreEqual(30, results[2].score);
        }
    }
}