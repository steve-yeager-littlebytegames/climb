using System;
using Climb.Data;
using Climb.Models;
using Climb.Services;
using Climb.Test.Utilities;
using NSubstitute;
using NUnit.Framework;

namespace Climb.Test.Models
{
    [TestFixture]
    public class SetTest
    {
        private ApplicationDbContext dbContext;
        private IDateService dateService;

        [SetUp]
        public void SetUp()
        {
            dbContext = DbContextUtility.CreateMockDb();
            dateService = Substitute.For<IDateService>();
        }

        [Test]
        public void Forfeit_HasPlayer_SetsUpdatedDate()
        {
            dateService.Now.Returns(DateTime.Today);
            var testObj = CreateTestObj();

            testObj.Forfeit(1, dateService);

            Assert.AreEqual(DateTime.Today, testObj.UpdatedDate);
        }

        private Set CreateTestObj()
        {
            var league = dbContext.CreateLeague(2);
            return SetUtility.Create(dbContext, league.Members[0].ID, league.Members[1].ID, league.ID);
        }
    }
}