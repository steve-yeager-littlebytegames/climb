using System.Threading.Tasks;
using Climb.Controllers;
using Climb.Data;
using Climb.Services;
using Climb.Services.ModelServices;
using Climb.Test.Utilities;
using Microsoft.AspNetCore.Hosting;
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
            var environment = Substitute.For<IHostingEnvironment>();
            var dateService = Substitute.For<IDateService>();

            testObj = new SeasonController(seasonService, dbContext, logger, userManager, environment, dateService);
        }

        [Test]
        public async Task Start_NotStarted_SetActive()
        {
            var season = SeasonUtility.CreateSeason(dbContext, 2).season;
            seasonService.GenerateSchedule(season.ID).Returns(season);

            await testObj.Start(season.ID);

            Assert.IsTrue(season.IsActive);
        }
        
        [Test]
        public async Task Start_NotStarted_SetLeagueActiveSeason()
        {
            var season = SeasonUtility.CreateSeason(dbContext, 2).season;
            seasonService.GenerateSchedule(season.ID).Returns(season);

            await testObj.Start(season.ID);

            Assert.AreEqual(season.ID, season.League.ActiveSeasonID);
        }
    }
}