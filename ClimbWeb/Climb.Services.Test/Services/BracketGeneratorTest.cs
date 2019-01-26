using System;
using Climb.Services;
using NUnit.Framework;

namespace Climb.Test.Services
{
    [TestFixture]
    public class BracketGeneratorTest
    {
        private BracketGenerator testObj;

        [SetUp]
        public void SetUp()
        {
            testObj = new BracketGenerator();
        }

        [Test]
        public void Generate_NotEnoughUsers_ThrowException()
        {
            var userCount = testObj.MinCompetitors - 1;

            Assert.Throws<ArgumentException>(() => testObj.Generate(userCount));
        }

        [TestCase(4)]
        [TestCase(16)]
        public void Generate_CreateCorrectAmountOfSetSlots(int competitorCount)
        {
            int expectedCount = competitorCount * 2 - 1;

            var tournament = testObj.Generate(competitorCount);

            Assert.AreEqual(expectedCount, tournament.GameCount);
        }

        // need to create byes

        // number of slots == (N - 1) * 2 + 1

        // keep placing of seeds to separate method that can be easier changed

        //https://www.npmjs.com/package/react-tournament-bracket
        //http://www.aropupu.fi/bracket/
    }
}