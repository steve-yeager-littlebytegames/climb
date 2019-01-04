using Climb.Services;
using NUnit.Framework;

namespace Climb.Test.Services
{
    [TestFixture]
    public class EloPointServiceTest
    {
        private EloPointService testObj;

        [SetUp]
        public void SetUp()
        {
            testObj = new EloPointService();
        }

        [TestCase(2000, 2000, true, 16)]
        [TestCase(2000, 2000, false, 16)]
        [TestCase(2134, 1567, true, 1)]
        [TestCase(2134, 1567, false, 31)]
        [TestCase(2000, 1859, true, 10)]
        [TestCase(2000, 1859, false, 22)]
        public void CalculatePointDeltas_Valid_CorrectPoints(int p1Elo, int p2Elo, bool p1Won, int pointDelta)
        {
            var (p1Points, p2Points) = testObj.CalculatePointDeltas(p1Elo, p2Elo, p1Won);

            if(p1Won)
            {
                Assert.AreEqual(pointDelta, p1Points);
                Assert.AreEqual(-pointDelta, p2Points);
            }
            else
            {
                Assert.AreEqual(-pointDelta, p1Points);
                Assert.AreEqual(pointDelta, p2Points);
            }
        }
    }
}