using System.Linq;
using Climb.Services;
using MoreLinq.Extensions;
using NSubstitute;
using NUnit.Framework;

namespace Climb.Test.Services.Analyzer
{
    [TestFixture]
    public class AnalyzerFactoryTest
    {
        private AnalyzerFactory testObj;

        [SetUp]
        public void SetUp()
        {
            var dateService = Substitute.For<IDateService>();

            testObj = new AnalyzerFactory(dateService);
        }

        [Test]
        public void CreateAnalyzers_HasNoDuplicates()
        {
            var analyzers = testObj.CreateAnalyzers();

            var distinctAnalyzers = analyzers.DistinctBy(a => a.GetType());

            Assert.AreEqual(distinctAnalyzers.Count(), analyzers.Count);
        }
    }
}