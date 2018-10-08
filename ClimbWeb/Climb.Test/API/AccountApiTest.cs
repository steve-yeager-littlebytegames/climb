using System.Net;
using System.Threading.Tasks;
using Climb.API;
using Climb.Data;
using Climb.Requests.Account;
using Climb.Services;
using Climb.Services.ModelServices;
using Climb.Test.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace Climb.Test.Controllers
{
    [TestFixture]
    public class AccountApiTest
    {
        private AccountApi testObj;
        private IApplicationUserService applicationUserService;
        private ISignInManager signInManager;

        [SetUp]
        public void SetUp()
        {
            signInManager = Substitute.For<ISignInManager>();
            var logger = Substitute.For<ILogger<AccountApi>>();
            var tokenHelper = Substitute.For<ITokenHelper>();
            applicationUserService = Substitute.For<IApplicationUserService>();
            var cdnService = Substitute.For<ICdnService>();

            testObj = new AccountApi(signInManager, tokenHelper, logger, applicationUserService, cdnService)
            {
                ControllerContext = {HttpContext = new DefaultHttpContext()},
            };
        }

        [Test]
        public async Task Register_Valid_Created()
        {
            var request = new RegisterRequest();
            applicationUserService.Register(request, Arg.Any<IUrlHelper>(), Arg.Any<string>()).Returns(new ApplicationUser());

            var result = await testObj.Register(request);

            ControllerUtility.AssertStatusCode(result, HttpStatusCode.Created);
        }

        [Test]
        public async Task LogIn_Valid_Ok()
        {
            signInManager.PasswordSignInAsync("", "", false, false).ReturnsForAnyArgs(SignInResult.Success);
            var request = new LoginRequest();

            var result = await testObj.LogIn(request);

            ControllerUtility.AssertStatusCode(result, HttpStatusCode.OK);
        }
    }
}