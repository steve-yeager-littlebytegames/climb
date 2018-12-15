using System.Collections.Generic;
using System.Threading.Tasks;
using Climb.Data;
using Climb.Exceptions;
using Climb.Services;
using Climb.Test.Utilities;
using NSubstitute;
using NUnit.Framework;

namespace Climb.Test.Services
{
    [TestFixture]
    public class AnalyzerServiceTest
    {
        private AnalyzerService testObj;
        private ApplicationDbContext dbContext;
        private IAnalyzerFactory analyzerFactory;

        [SetUp]
        public void SetUp()
        {
            dbContext = DbContextUtility.CreateMockDb();
            var dateService = Substitute.For<IDateService>();
            analyzerFactory = Substitute.For<IAnalyzerFactory>();

            testObj = new AnalyzerService(dbContext, dateService, analyzerFactory);
        }

        [TestCase(0, 1)]
        [TestCase(1, 0)]
        public void Calculate_PlayerNotFound_NotFoundException(int p1IDOffset1, int p2IDOffset)
        {
            var memberID = dbContext.CreateLeague(1).Members[0].ID;

            Assert.ThrowsAsync<NotFoundException>(() => testObj.Calculate(memberID + p1IDOffset1, memberID + p2IDOffset));
        }

        [Test]
        public async Task Calculate_GetAnalyzers_RunAllAnalyzers()
        {
            var analyzers = CreateAnalyzers(3);

            analyzerFactory.CreateAnalyzers().Returns(analyzers);
            var members = dbContext.CreateLeague(2).Members;

            var dataCollection = await testObj.Calculate(members[0].ID, members[1].ID);

            Assert.IsNotNull(dataCollection);
            foreach(var analyzer in analyzers)
            {
#pragma warning disable 4014
                analyzer.Received(1).Analyze(members[0].ID, members[1].ID, dbContext);
#pragma warning restore 4014
            }
        }
        
        [Test]
        public async Task Calculate_NullAnalyzerData_DontCollectNullData()
        {
            var analyzers = CreateAnalyzers(3);
            analyzers[0].Analyze(0, 0, null).ReturnsForAnyArgs(new string[1]);
            analyzers[2].Analyze(0, 0, null).ReturnsForAnyArgs(new string[1]);

            analyzerFactory.CreateAnalyzers().Returns(analyzers);
            var members = dbContext.CreateLeague(2).Members;

            var dataCollection = await testObj.Calculate(members[0].ID, members[1].ID);

            Assert.AreEqual(2, dataCollection.Count);
        }

        private static List<DataAnalyzer> CreateAnalyzers(int count)
        {
            var analyzers = new List<DataAnalyzer>();
            for(int i = 0; i < count; i++)
            {
                analyzers.Add(Substitute.For<DataAnalyzer>());
            }

            return analyzers;
        }
    }
}