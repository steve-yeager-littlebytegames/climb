using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Climb.Core.TieBreakers.Test
{
    [TestFixture]
    public class MembershipDurationRuleTest
    {
        private MembershipDurationRule testObj;

        [SetUp]
        public void SetUp()
        {
            testObj = new MembershipDurationRule(DateTime.Today);
        }

        [Test]
        public void Evaluate_ReturnMembershipSeconds()
        {
            Dictionary<IParticipant, ParticipantRecord> participants = ParticipantsBuilder.Create()
                .Add(joinDate: DateTime.Today.AddDays(-3 * 7))
                .Add(joinDate: DateTime.Today.AddDays(-4 * 7))
                .Add(joinDate: DateTime.Today.AddDays(-5 * 7));

            var results = testObj.Evaluate(participants);

            Assert.AreEqual(3 * 7 * 24 * 60 * 60, results[0].score);
            Assert.AreEqual(4 * 7 * 24 * 60 * 60, results[1].score);
            Assert.AreEqual(5 * 7 * 24 * 60 * 60, results[2].score);
        }
    }
}