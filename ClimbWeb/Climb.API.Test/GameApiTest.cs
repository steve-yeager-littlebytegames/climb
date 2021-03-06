﻿using System.Net;
using System.Threading.Tasks;
using Climb.API;
using Climb.Data;
using Climb.Extensions;
using Climb.Requests.Game;
using Climb.Responses.Models;
using Climb.Services;
using Climb.Services.ModelServices;
using Climb.Test.Utilities;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;

namespace Climb.Test.Api
{
    [TestFixture]
    public class GameApiTest
    {
        private GameApi testObj;
        private ApplicationDbContext dbContext;

        [SetUp]
        public void SetUp()
        {
            dbContext = DbContextUtility.CreateMockDb();
            var logger = Substitute.For<ILogger<GameApi>>();
            var cdnService = Substitute.For<ICdnService>();
            var gameService = Substitute.For<IGameService>();

            testObj = new GameApi(logger, dbContext, cdnService, gameService);
        }

        [Test]
        public async Task Get_HasGame_Ok()
        {
            var game = dbContext.CreateGame(2, 2);

            var result = await testObj.Get(game.ID);
            var resultObj = result.GetObject<GameDto>();

            ControllerUtility.AssertStatusCode(result, HttpStatusCode.OK);
            Assert.AreEqual(game.ID, resultObj.ID);
        }

        [Test]
        public async Task Get_NoGame_NotFound()
        {
            var result = await testObj.Get(0);

            ControllerUtility.AssertStatusCode(result, HttpStatusCode.NotFound);
        }

        [Test]
        public async Task AddCharacters_NullNames_BadRequest()
        {
            var request = new AddCharacters(1, null);

            var result = await testObj.AddCharacters(request);

            ControllerUtility.AssertStatusCode(result, HttpStatusCode.BadRequest);
        }
    }
}