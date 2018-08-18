using System.Threading.Tasks;
using Climb.Controllers;
using Climb.Data;
using Climb.Services;
using Climb.Services.ModelServices;
using Climb.Test.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using NSubstitute;
using NUnit.Framework;

namespace Climb.Test.Controllers
{
    [TestFixture]
    public class AccountControllerTest
    {
        private AccountController testObj;
        private IUserManager userManager;
        private ApplicationDbContext dbContext;

        [SetUp]
        public void SetUp()
        {
            var logger = Substitute.For<ILogger<AccountController>>();
            var applicationUserService = Substitute.For<IApplicationUserService>();
            userManager = Substitute.For<IUserManager>();
            dbContext = DbContextUtility.CreateMockDb();
            var signInManager = Substitute.For<ISignInManager>();
            var cdnService = Substitute.For<ICdnService>();
            var emailSender = Substitute.For<IEmailSender>();

            testObj = new AccountController(logger, applicationUserService, userManager, dbContext, signInManager, cdnService, emailSender);
        }

        [Test]
        public async Task ForgotPassword_SignedIn_RedirectToUserHome()
        {
            var user = DbContextUtility.AddNew<ApplicationUser>(dbContext);
            userManager.GetUserAsync(null).ReturnsForAnyArgs(user);

            var result = await testObj.ForgotPassword();

            Assert.IsInstanceOf<RedirectToActionResult>(result);
        }

        [Test]
        public async Task ResetPassword_PasswordsDontMatch_BadRequest()
        {
            var user = DbContextUtility.AddNew<ApplicationUser>(dbContext);
            const string password = "abc";
            const string token = "";
            userManager.ResetPasswordAsync(user, token, password).Returns(IdentityResult.Success);

            var result = await testObj.ResetPasswordPost(user.Id, token, password, "123");

            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }
        
        [Test]
        public async Task ResetPassword_PasswordsMatch_Redirect()
        {
            var user = DbContextUtility.AddNew<ApplicationUser>(dbContext);
            const string password = "abc";
            const string token = "";
            userManager.ResetPasswordAsync(user, token, password).Returns(IdentityResult.Success);

            var result = await testObj.ResetPasswordPost(user.Id, token, password, password);

            Assert.IsInstanceOf<RedirectToActionResult>(result);
        }

        [Test]
        public async Task ResetPassword_InvalidToken_BadRequest()
        {
            var user = DbContextUtility.AddNew<ApplicationUser>(dbContext);
            const string password = "abc";
            const string token = "";
            userManager.ResetPasswordAsync(user, token, password).Returns(IdentityResult.Failed());

            var result = await testObj.ResetPasswordPost(user.Id, token, password, password);

            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        [Test]
        public async Task ResetPassword_NoUser_BadRequest()
        {
            const string userID = "";
            const string password = "abc";
            const string token = "";

            var result = await testObj.ResetPasswordPost(userID, token, password, password);

            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }
    }
}