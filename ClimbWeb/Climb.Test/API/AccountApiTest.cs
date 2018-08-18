using System.Net;
using System.Threading.Tasks;
using Climb.API;
using Climb.Requests.Account;
using Climb.Services;
using Climb.Services.ModelServices;
using Climb.Test.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;

namespace Climb.Test.Controllers
{
    [TestFixture]
    public class AccountApiTest
    {
        private AccountApi testObj;
        private IUserManager userManager;
        private ISignInManager signInManager;

        [SetUp]
        public void SetUp()
        {
            userManager = Substitute.For<IUserManager>();
            signInManager = Substitute.For<ISignInManager>();
            var logger = Substitute.For<ILogger<AccountApi>>();
            var tokenHelper = Substitute.For<ITokenHelper>();
            var applicationUserService = Substitute.For<IApplicationUserService>();

            testObj = new AccountApi(signInManager, tokenHelper, logger, applicationUserService)
            {
                ControllerContext = {HttpContext = new DefaultHttpContext()},
            };
        }

        [Test]
        public async Task Register_Valid_Created()
        {
            userManager.CreateAsync(null, null).ReturnsForAnyArgs(IdentityResult.Success);

            var request = new RegisterRequest();

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