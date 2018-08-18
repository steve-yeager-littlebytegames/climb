using System.Threading.Tasks;
using Climb.Controllers;
using Climb.Data;
using Climb.Services;
using Climb.Services.ModelServices;
using Climb.Test.Utilities;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;

namespace Climb.Test.Controllers
{
    [TestFixture]
    public class SeasonControllerTest
    {
        private SeasonController testObj;
        private ISeasonService seasonService;
        private ApplicationDbContext dbContext;

        [SetUp]
        public void SetUp()
        {
            seasonService = Substitute.For<ISeasonService>();
            dbContext = DbContextUtility.CreateMockDb();
            var logger = Substitute.For<ILogger<SeasonController>>();
            var userManager = Substitute.For<IUserManager>();

            testObj = new SeasonController(seasonService, dbContext, logger, userManager);
        }

        [Test]
        public async Task Start_SeasonExists_SeasonIsActive()
        {
            var season = SeasonUtility.CreateSeason(dbContext, 2).season;
            seasonService.GenerateSchedule(season.ID).Returns(season);

            await testObj.Start(season.ID);

            Assert.IsTrue(season.IsActive);
        }
    }
}