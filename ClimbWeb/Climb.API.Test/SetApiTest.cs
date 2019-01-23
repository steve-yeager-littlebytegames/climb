using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Climb.API;
using Climb.Data;
using Climb.Extensions;
using Climb.Models;
using Climb.Requests.Sets;
using Climb.Responses.Sets;
using Climb.Services.ModelServices;
using Climb.Test.Utilities;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;

namespace Climb.Test.Api
{
    [TestFixture]
    public class SetApiTest
    {
        private SetApi testObj;
        private ISetService setService;
        private ApplicationDbContext dbContext;

        [SetUp]
        public void SetUp()
        {
            setService = Substitute.For<ISetService>();
            dbContext = DbContextUtility.CreateMockDb();
            var logger = Substitute.For<ILogger<SetApi>>();

            testObj = new SetApi(logger, dbContext, setService);
        }

        [Test]
        public async Task Get_HasSet_Ok()
        {
            var set = SetUtility.Create(dbContext);

            var result = await testObj.Get(set.ID);
            var resultObj = result.GetObject<SetDto>();

            ControllerUtility.AssertStatusCode(result, HttpStatusCode.OK);
            Assert.AreEqual(set.ID, resultObj.ID);
        }

        [Test]
        public async Task Get_NoSet_NotFound()
        {
            var result = await testObj.Get(0);

            ControllerUtility.AssertStatusCode(result, HttpStatusCode.NotFound);
        }

        [Test]
        public async Task Submit_Valid_Ok()
        {
            var request = CreateSet().request;

            var result = await testObj.Submit(request);

            ControllerUtility.AssertStatusCode(result, HttpStatusCode.OK);
        }

        [Test]
        public async Task Submit_Valid_ReturnSetDto()
        {
            var (set, request) = CreateSet();

            var result = await testObj.Submit(request);
            var resultObj = result.GetObject<SetDto>();

            Assert.AreEqual(set.ID, resultObj.ID);
        }

        private (Set set, SubmitRequest request) CreateSet()
        {
            var set = SetUtility.Create(dbContext);
            set.Matches = new List<Match>();
            setService.Update(set.ID, Arg.Any<MatchForm[]>()).Returns(set);
            var request = new SubmitRequest
            {
                SetID = set.ID
            };

            return (set, request);
        }
    }
}