using Climb.Services;
using NUnit.Framework;

namespace Climb.Test.Services
{
    [TestFixture]
    public class ParticipationSeasonPointCalculatorTest
    {
        private ParticipationSeasonPointCalculator testObj;

        [SetUp]
        public void SetUp()
        {
            testObj = new ParticipationSeasonPointCalculator();
        }

        [Test]
        public void CalculatePoints_Valid_WinnerGets2LoserGets1()
        {
            var (winnerDelta, loserDelta) = testObj.CalculatePointDeltas(null, null);

            Assert.AreEqual(2, winnerDelta);
            Assert.AreEqual(1, loserDelta);
        }
    }
}