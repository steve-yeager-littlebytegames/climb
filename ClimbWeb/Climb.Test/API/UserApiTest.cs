using System.Net;
using System.Threading.Tasks;
using Climb.API;
using Climb.Data;
using Climb.Extensions;
using Climb.Responses;
using Climb.Services;
using Climb.Services.ModelServices;
using Climb.Test.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;

namespace Climb.Test.Api
{
    [TestFixture]
    public class UserApiTest
    {
        private UserApi testObj;
        private IApplicationUserService applicationUserService;
        private ApplicationDbContext dbContext;

        [SetUp]
        public void SetUp()
        {
            dbContext = DbContextUtility.CreateMockDb();
            applicationUserService = Substitute.For<IApplicationUserService>();
            var logger = Substitute.For<ILogger<UserApi>>();
            var cdnService = Substitute.For<ICdnService>();

            testObj = new UserApi(logger, dbContext, cdnService, applicationUserService);
        }

        [Test]
        public async Task Get_Valid_Ok()
        {
            var user = dbContext.AddNew<ApplicationUser>(u => u.Email = "user@email.com");

            var result = await testObj.Get(user.Id);

            ControllerUtility.AssertStatusCode(result, HttpStatusCode.OK);
        }

        [Test]
        public async Task Get_Valid_ReturnUser()
        {
            var user = dbContext.AddNew<ApplicationUser>(u => u.Email = "user@email.com");

            var result = await testObj.Get(user.Id);
            var resultObj = result.GetObject<UserDto>();

            Assert.AreEqual(user.Email, resultObj.Email);
        }

        [Test]
        public async Task Get_NoUser_NotFound()
        {
            var result = await testObj.Get("");

            ControllerUtility.AssertStatusCode(result, HttpStatusCode.NotFound);
        }

        [Test]
        public async Task UploadProfilePic_Valid_Created()
        {
            var user = dbContext.AddNew<ApplicationUser>();
            var file = Substitute.For<IFormFile>();

            var result = await testObj.UploadProfilePic(user.Id, file);

            ControllerUtility.AssertStatusCode(result, HttpStatusCode.Created);
        }

        [Test]
        public async Task UploadProfilePic_Valid_ReturnUrl()
        {
            const string imageUrl = "www.image.com";
            var user = dbContext.AddNew<ApplicationUser>();
            var file = Substitute.For<IFormFile>();
            applicationUserService.UploadProfilePic(user.Id, file).Returns(imageUrl);

            var result = await testObj.UploadProfilePic(user.Id, file);
            var resultObj = result.GetObject<string>();

            Assert.AreEqual(imageUrl, resultObj);
        }
    }
}