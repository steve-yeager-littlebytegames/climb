using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Climb.API;
using Climb.Data;
using Climb.Extensions;
using Climb.Models;
using Climb.Requests.Leagues;
using Climb.Responses.Models;
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

            testObj = new LeagueApi(logger, dbContext, leagueService, configuration);
        }

        [Test]
        public async Task Create_Valid_CreatedResult()
        {
            var gameID = DbContextUtility.AddNew<Game>(dbContext).ID;
            var request = new CreateRequest
            {
                Name = LeagueName,
                GameID = gameID
            };

            leagueService.Create(LeagueName, gameID, "").Returns(new League
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
            var league = LeagueUtility.CreateLeague(dbContext);
            var user = DbContextUtility.AddNew<ApplicationUser>(dbContext);

            var request = new JoinRequest(league.ID, user.Id);

            var result = await testObj.Join(request);

            ControllerUtility.AssertStatusCode(result, HttpStatusCode.Created);
        }

        [Test]
        public async Task Get_Valid_Ok()
        {
            var league = LeagueUtility.CreateLeague(dbContext);

            var result = await testObj.Get(league.ID);
            var resultLeague = result.GetObject<League>();

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
            var league = LeagueUtility.CreateLeague(dbContext);
            var leagueUser = LeagueUtility.AddUsersToLeague(league, 1, dbContext)[0];

            var result = await testObj.GetUser(leagueUser.ID);
            var resultObj = result.GetObject<LeagueUserDto>();

            ControllerUtility.AssertStatusCode(result, HttpStatusCode.OK);
            Assert.AreEqual(leagueUser.ID, resultObj.ID);
        }

        [Test]
        public async Task GetUser_NoUser_NotFound()
        {
            var result = await testObj.GetUser(0);

            ControllerUtility.AssertStatusCode(result, HttpStatusCode.NotFound);
        }

        [Test]
        public async Task GetSeasons_Valid_Ok()
        {
            var league = LeagueUtility.CreateLeague(dbContext);

            var result = await testObj.GetSeasons(league.ID);

            ControllerUtility.AssertStatusCode(result, HttpStatusCode.OK);
        }

        [Test]
        public async Task GetSeasons_Valid_ReturnsSeasons()
        {
            var league = LeagueUtility.CreateLeague(dbContext);
            DbContextUtility.AddNew<Season>(dbContext, s => s.LeagueID = league.ID);
            DbContextUtility.AddNew<Season>(dbContext, s => s.LeagueID = league.ID);

            var result = await testObj.GetSeasons(league.ID);
            var seasons = result.GetObject<ICollection<Season>>();

            Assert.AreEqual(2, seasons.Count);
        }

        [Test]
        public async Task GetSeasons_NoLeague_NotFound()
        {
            var result = await testObj.GetSeasons(0);

            ControllerUtility.AssertStatusCode(result, HttpStatusCode.NotFound);
        }

        //[Test]
        //public async Task GetSets_NoLeagueUser_NotFound()
        //{
        //    var result = await testObj.GetSets(0);

        //    ControllerUtility.AssertStatusCode(result, HttpStatusCode.NotFound);
        //}

        //[Test]
        //public async Task GetSets_HasSets_Ok()
        //{
        //    var members = CreateMembersWithSets(2);

        //    var result = await testObj.GetSets(members[0].ID);

        //    ControllerUtility.AssertStatusCode(result, HttpStatusCode.OK);
        //}

        //[TestCase(0)]
        //[TestCase(1)]
        //[TestCase(10)]
        //public async Task GetSets_HasSets_ReturnSets(int setCount)
        //{
        //    var members = CreateMembersWithSets(setCount);

        //    var result = await testObj.GetSets(members[0].ID);
        //    var resultObj = result.GetObject<SetDto[]>();

        //    Assert.AreEqual(setCount, resultObj.Length);
        //}

        private List<LeagueUser> CreateMembersWithSets(int setCount)
        {
            var league = LeagueUtility.CreateLeague(dbContext);
            var members = LeagueUtility.AddUsersToLeague(league, 2, dbContext);

            for(var i = 0; i < setCount; ++i)
            {
                SetUtility.Create(dbContext, members[0].ID, members[1].ID, league.ID);
            }

            return members;
        }
    }
}