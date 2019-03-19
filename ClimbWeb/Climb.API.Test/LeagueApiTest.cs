using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Climb.API;
using Climb.Data;
using Climb.Extensions;
using Climb.Models;
using Climb.Requests.Leagues;
using Climb.Responses.Models;
using Climb.Services;
using Climb.Services.ModelServices;
using Climb.Test.Utilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;

namespace Climb.Test.Api
{
    [TestFixture]
    public class LeagueApiTest
    {
        private const string LeagueName = "NewLeague";

        private LeagueApi testObj;
        private ILeagueService leagueService;
        private ApplicationDbContext dbContext;

        [SetUp]
        public void SetUp()
        {
            leagueService = Substitute.For<ILeagueService>();
            dbContext = DbContextUtility.CreateMockDb();
            var logger = Substitute.For<ILogger<LeagueApi>>();
            var configuration = Substitute.For<IConfiguration>();
            var cdnService = Substitute.For<ICdnService>();
            var dateService = Substitute.For<IDateService>();

            testObj = new LeagueApi(logger, dbContext, leagueService, configuration, cdnService, dateService);
        }

        [Test]
        public async Task Create_Valid_CreatedResult()
        {
            var gameID = dbContext.AddNew<Game>().ID;
            var request = new CreateRequest
            {
                Name = LeagueName,
                GameID = gameID
            };

            leagueService.Create(LeagueName, gameID, Arg.Any<string>()).Returns(new League
            {
                Name = LeagueName,
                GameID = gameID
            });

            var result = await testObj.Create(request);

            ControllerUtility.AssertStatusCode(result, HttpStatusCode.Created);
        }

        [Test]
        public async Task Join_Valid_Created()
        {
            leagueService.Join(1, "ID").Returns(new LeagueUser());

            var request = new JoinRequest(1, "ID");

            var result = await testObj.Join(request);

            ControllerUtility.AssertStatusCode(result, HttpStatusCode.Created);
        }

        [Test]
        public async Task Get_Valid_Ok()
        {
            var league = dbContext.CreateLeague();

            var result = await testObj.Get(league.ID);
            var resultLeague = result.GetObject<LeagueDto>();

            ControllerUtility.AssertStatusCode(result, HttpStatusCode.OK);
            Assert.IsNotNull(resultLeague);
        }

        [Test]
        public async Task Get_NoLeague_NotFound()
        {
            var result = await testObj.Get(0);

            ControllerUtility.AssertStatusCode(result, HttpStatusCode.NotFound);
        }

        [Test]
        public async Task GetUser_HasUser_Ok()
        {
            var league = dbContext.CreateLeague();
            var leagueUser = LeagueUtility.AddUsersToLeague(league, 1, dbContext)[0];

            var result = await testObj.GetMember(leagueUser.ID);
            var resultObj = result.GetObject<LeagueUserDto>();

            ControllerUtility.AssertStatusCode(result, HttpStatusCode.OK);
            Assert.AreEqual(leagueUser.ID, resultObj.ID);
        }

        [Test]
        public async Task GetUser_NoUser_NotFound()
        {
            var result = await testObj.GetMember(0);

            ControllerUtility.AssertStatusCode(result, HttpStatusCode.NotFound);
        }

        [Test]
        public async Task GetSeasons_Valid_Ok()
        {
            var league = dbContext.CreateLeague();

            var result = await testObj.GetSeasons(league.ID);

            ControllerUtility.AssertStatusCode(result, HttpStatusCode.OK);
        }

        [Test]
        public async Task GetSeasons_Valid_ReturnsSeasons()
        {
            var league = dbContext.CreateLeague();
            dbContext.AddNew<Season>(s => s.LeagueID = league.ID);
            dbContext.AddNew<Season>(s => s.LeagueID = league.ID);

            var result = await testObj.GetSeasons(league.ID);
            var seasons = result.GetObject<IEnumerable<SeasonDto>>();

            Assert.AreEqual(2, seasons.Count());
        }

        [Test]
        public async Task GetSeasons_NoLeague_NotFound()
        {
            var result = await testObj.GetSeasons(0);

            ControllerUtility.AssertStatusCode(result, HttpStatusCode.NotFound);
        }
    }
}