using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Climb.Core.TieBreakers.Test
{
    [TestFixture]
    public class TieBreakerTests
    {
        private class FakeRule : TieBreakerRule
        {
            private readonly int[] scores;
            private int index = -1;

            public FakeRule(params int[] scores)
            {
                this.scores = scores;
            }

            protected override int GetParticipantScore((IParticipant participant, ParticipantRecord record) participant, IReadOnlyDictionary<IParticipant, ParticipantRecord> tiedParticipants)
            {
                ++index;
                return scores[index];
            }
        }

        private class Fake2Rule : FakeRule
        {
            public Fake2Rule(params int[] scores)
                : base(scores)
            {
            }
        }

        private class Fake3Rule : FakeRule
        {
            public Fake3Rule(params int[] scores)
                : base(scores)
            {
            }
        }

        private TieBreaker testObj;

        [SetUp]
        public void SetUp()
        {
            testObj = new TieBreaker();
        }

        [Test]
        public void AddRule_Null_ArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => testObj.AddRule(null));
        }

        [Test]
        public void AddRule_Duplicate_ArgumentException()
        {
            var rule = new FakeRule();
            testObj.AddRule(rule);
            var rule2 = new FakeRule();

            Assert.Throws<ArgumentException>(() => testObj.AddRule(rule2));
        }

        [Test]
        public void Break_1Rule_ParticipantTieBreakerScoreUpdated()
        {
            // 3rd, 1st, 2nd
            var scores = new[] { 2, 14, 7 };
            testObj.AddRule(new FakeRule(scores));
            var participants = CreateParticipants();

            testObj.Break(participants);
            var sortedParticipants = participants.Select(p => p.Key).OrderByDescending(p => p.TieBreakerPoints).ToList();

            Assert.AreEqual(2, sortedParticipants[0].ID);
            Assert.AreEqual(3, sortedParticipants[1].ID);
            Assert.AreEqual(1, sortedParticipants[2].ID);
        }

        [Test]
        public void Break_MultipleRules_FirstRuleWins()
        {
            // 3rd, 1st, 2nd
            var scores = new[] { 2, 14, 7 };
            testObj.AddRule(new FakeRule(scores));
            scores = new[] { 200, 14, 7 };
            testObj.AddRule(new Fake2Rule(scores));
            scores = new[] { 200, 14, 7 };
            testObj.AddRule(new Fake3Rule(scores));

            var participants = CreateParticipants();

            testObj.Break(participants);
            var sortedParticipants = participants.Select(p => p.Key).OrderByDescending(p => p.TieBreakerPoints).ToList();

            Assert.AreEqual(2, sortedParticipants[0].ID);
            Assert.AreEqual(3, sortedParticipants[1].ID);
            Assert.AreEqual(1, sortedParticipants[2].ID);
        }

        private static Dictionary<IParticipant, ParticipantRecord> CreateParticipants()
        {
            return ParticipantsBuilder.Create()
                .Add(1)
                .Add(2)
                .Add(3);
        }
    }
}