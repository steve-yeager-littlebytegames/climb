using System;
using NUnit.Framework;

namespace Climb.Core.TieBreakers.Test
{
    [TestFixture]
    public class TieBreakerFactoryTests
    {
        private TieBreakerFactory testObj;

        [SetUp]
        public void SetUp()
        {
            testObj = new TieBreakerFactory();
        }

        [Test]
        public void Create_ReturnsTieBreaker()
        {
            var tieBreaker = testObj.Create(DateTime.Now);

            Assert.IsNotNull(tieBreaker);
        }
    }
}