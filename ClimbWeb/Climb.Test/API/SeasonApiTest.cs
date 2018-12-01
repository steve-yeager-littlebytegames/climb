using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Climb.API;
using Climb.Data;
using Climb.Extensions;
using Climb.Models;
using Climb.Requests.Seasons;
using Climb.Responses.Models;
using Climb.Responses.Sets;
using Climb.Services;
using Climb.Services.ModelServices;
using Climb.Test.Utilities;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;

namespace Climb.Test.Api
{
    [TestFixture]
    public class SeasonApiTest
    {
        private SeasonApi testObj;
        private ISeasonService seasonService;
        private ApplicationDbContext dbContext;

        [SetUp]
        public void SetUp()
        {
            dbContext = DbContextUtility.CreateMockDb();
            var game = dbContext.AddNew<Game>();
            gameID = game.ID;

            seasonService = Substitute.For<ISeasonService>();
            var logger = Substitute.For<ILogger<SeasonApi>>();
            var cdnService = Substitute.For<ICdnService>();

            testObj = new SeasonApi(logger, dbContext, seasonService, cdnService);
        }

        [Test]
        public async Task Get_Valid_Ok()
        {
            var season = SeasonUtility.CreateSeason(dbContext, 2).season;

            var result = await testObj.Get(season.ID);
            var resultObj = result.GetObject<SeasonDto>();

            ControllerUtility.AssertStatusCode(result, HttpStatusCode.OK);
            Assert.IsNotNull(resultObj);
        }

        [Test]
        public async Task Get_NoSeason_NotFound()
        {
            var result = await testObj.Get(0);

            ControllerUtility.AssertStatusCode(result, HttpStatusCode.NotFound);
        }

        [Test]
        public async Task Sets_HasSets_Ok()
        {
            var season = SeasonUtility.CreateSeason(dbContext, 2).season;
            var sets = SeasonUtility.CreateSets(dbContext, season);

            var result = await testObj.Sets(season.ID);
            var resultObj = result.GetObject<IEnumerable<SetDto>>();

            ControllerUtility.AssertStatusCode(result, HttpStatusCode.OK);
            Assert.AreEqual(sets.Count, resultObj.Count());
        }

        [Test]
        public async Task Sets_NoSets_Ok()
        {
            var season = SeasonUtility.CreateSeason(dbContext, 2).season;

            var result = await testObj.Sets(season.ID);
            var resultObj = result.GetObject<IEnumerable<SetDto>>();

            ControllerUtility.AssertStatusCode(result, HttpStatusCode.OK);
            Assert.IsNotNull(resultObj);
        }

        [Test]
        public async Task Sets_NoSeason_NotFound()
        {
            var result = await testObj.Sets(0);

            ControllerUtility.AssertStatusCode(result, HttpStatusCode.NotFound);
        }

        [TestCase(0)]
        [TestCase(2)]
        public async Task Participants_Valid_Ok(int participantsCount)
        {
            var season = SeasonUtility.CreateSeason(dbContext, participantsCount).season;

            var result = await testObj.Participants(season.ID);
            var resultObj = result.GetObject<IEnumerable<SeasonLeagueUserDto>>();

            ControllerUtility.AssertStatusCode(result, HttpStatusCode.OK);
            Assert.AreEqual(participantsCount, resultObj.Count());
        }

        [Test]
        public async Task Participants_NoSeason_NotFound()
        {
            var result = await testObj.Participants(0);

            ControllerUtility.AssertStatusCode(result, HttpStatusCode.NotFound);
        }

        [Test]
        public async Task Create_Valid_ReturnOk()
        {
            var request = new CreateRequest(1, DateTime.MinValue, DateTime.MaxValue);
            seasonService.Create(request.LeagueID, request.StartDate, request.EndDate).Returns(new Season());

            var result = await testObj.Create(request);

            ControllerUtility.AssertStatusCode(result, HttpStatusCode.Created);
        }

        [Test]
        public async Task Start_Valid_Created()
        {
            var season = SeasonUtility.CreateSeason(dbContext, 2).season;
            SeasonUtility.CreateSets(dbContext, season);
            seasonService.GenerateSchedule(1).Returns(season);

            var result = await testObj.Start(season.ID);

            ControllerUtility.AssertStatusCode(result, HttpStatusCode.Created);
        }
    }
}