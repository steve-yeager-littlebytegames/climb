using System.Collections.Generic;
using NUnit.Framework;

namespace Climb.Core.TieBreakers.Test
{
    [TestFixture]
    public class TieBreakAttemptTests
    {
        private class FakeTieBreakRule : TieBreakerRule
        {
            public readonly List<IParticipant> participants = new List<IParticipant>();

            protected override int GetParticipantScore((IParticipant participant, ParticipantRecord record) participant, IReadOnlyDictionary<IParticipant, ParticipantRecord> tiedParticipants)
            {
                participants.Add(participant.participant);
                return 0;
            }
        }

        private FakeTieBreakRule testObj;

        [SetUp]
        public void SetUp()
        {
            testObj = new FakeTieBreakRule();
        }

        [Test]
        public void Evaluate_AllUsersEvaulated()
        {
            Dictionary<IParticipant, ParticipantRecord> participants = ParticipantsBuilder.Create()
                .Add()
                .Add()
                .Add();

            testObj.Evaluate(participants);

            foreach(var participant in participants)
            {
                testObj.participants.Remove(participant.Key);
            }

            Assert.AreEqual(0, testObj.participants.Count);
        }
    }
}