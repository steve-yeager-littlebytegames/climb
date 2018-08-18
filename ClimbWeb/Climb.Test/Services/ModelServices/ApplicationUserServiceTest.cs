using System.Threading.Tasks;
using Climb.Data;
using Climb.Exceptions;
using Climb.Requests.Account;
using Climb.Services;
using Climb.Services.ModelServices;
using Climb.Test.Utilities;
using Climb.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using NUnit.Framework;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace Climb.Test.Services.ModelServices
{
    [TestFixture]
    public class ApplicationUserServiceTest
    {
        private const string ImageKey = "KEY";
        private const string ImageUrl = "IMAGEURL";
        private ApplicationUserService testObj;
        private ApplicationDbContext dbContext;
        private ICdnService cdnService;
        private IUserManager userManager;
        private ISignInManager signInManager;

        [SetUp]
        public void SetUp()
        {
            dbContext = DbContextUtility.CreateMockDb();
            cdnService = Substitute.For<ICdnService>();

            var configuration = Substitute.For<IConfiguration>();
            configuration["SecurityKey"].Returns("key");

            var emailSender = Substitute.For<IEmailSender>();
            var tokenHelper = Substitute.For<ITokenHelper>();
            var urlUtility = Substitute.For<IUrlUtility>();
            signInManager = Substitute.For<ISignInManager>();
            userManager = Substitute.For<IUserManager>();

            testObj = new ApplicationUserService(dbContext, cdnService, signInManager, emailSender, configuration, tokenHelper, urlUtility, userManager);
        }

        [Test]
        public void LogIn_WrongPassword_BadRequestException()
        {
            signInManager.PasswordSignInAsync("", "", false, false).ReturnsForAnyArgs(SignInResult.Failed);
            const string email = "user@test.com";
            DbContextUtility.AddNew<ApplicationUser>(dbContext, u => u.Email = email);
            var request = new LoginRequest {Email = email};

            Assert.ThrowsAsync<BadRequestException>(() => testObj.LogIn(request));
        }

        [Test]
        public void Register_Fail_BadRequestException()
        {
            userManager.CreateAsync(null, null).ReturnsForAnyArgs(IdentityResult.Failed());
            var request = new RegisterRequest();
            var urlHelper = Substitute.For<IUrlHelper>();

            Assert.ThrowsAsync<BadRequestException>(() => testObj.Register(request, urlHelper, ""));
        }

        [Test]
        public async Task Register_Valid_SetValues()
        {
            userManager.CreateAsync(null, null).ReturnsForAnyArgs(IdentityResult.Success);
            const string name = "bob";
            var request = new RegisterRequest {Name = name};
            var urlHelper = Substitute.For<IUrlHelper>();

            var user = await testObj.Register(request, urlHelper, "");

            Assert.AreEqual(name, user.Name);
        }

        [Test]
        public async Task UploadImage_Valid_SetsUserProfilePicKey()
        {
            var user = DbContextUtility.AddNew<ApplicationUser>(dbContext);
            var file = PrepareCdnService();

            await testObj.UploadProfilePic(user.Id, file);

            Assert.AreEqual(ImageKey, user.ProfilePicKey);
        }

        [Test]
        public async Task UploadImage_Valid_ReturnImageUrl()
        {
            var user = DbContextUtility.AddNew<ApplicationUser>(dbContext);
            var file = PrepareCdnService();

            var imageUrl = await testObj.UploadProfilePic(user.Id, file);

            Assert.AreEqual(ImageUrl, imageUrl);
        }

        [Test]
        public void UploadImage_NoUser_NotFoundException()
        {
            var file = PrepareCdnService();

            Assert.ThrowsAsync<NotFoundException>(() => testObj.UploadProfilePic("", file));
        }

        [Test]
        public void UploadImage_NoFile_BadRequestException()
        {
            var user = DbContextUtility.AddNew<ApplicationUser>(dbContext);

            Assert.ThrowsAsync<BadRequestException>(() => testObj.UploadProfilePic(user.Id, null));
        }

        [Test]
        public async Task UpdateSettings_NewValues_ValuesUpdated()
        {
            var user = DbContextUtility.AddNew<ApplicationUser>(dbContext);
            const string username = "bob";
            const string name = "ted";
            var file = Substitute.For<IFormFile>();

            await testObj.UpdateSettings(user.Id, username, name, file);

            Assert.AreEqual(username, user.UserName);
            Assert.AreEqual(name, user.Name);
        }

        [Test]
        public void UpdateSettings_NoUser_NotFoundException()
        {
            var file = Substitute.For<IFormFile>();

            Assert.ThrowsAsync<NotFoundException>(() => testObj.UpdateSettings("", "bob", "", file));
        }

        [Test]
        public async Task UpdateSettings_ProfilePic_UpdatePicture()
        {
            var user = DbContextUtility.AddNew<ApplicationUser>(dbContext);
            var file = Substitute.For<IFormFile>();

            await testObj.UpdateSettings(user.Id, "bob", "", file);

#pragma warning disable 4014
            cdnService.Received(1).ReplaceImageAsync(Arg.Any<string>(), file, ClimbImageRules.ProfilePic);
#pragma warning restore 4014
        }

        [Test]
        public async Task UpdateSettings_NoProfilePic_DontUpdatePicture()
        {
            var user = DbContextUtility.AddNew<ApplicationUser>(dbContext);

            await testObj.UpdateSettings(user.Id, "bob", "", null);

#pragma warning disable 4014
            cdnService.DidNotReceiveWithAnyArgs().UploadImageAsync(null, null);
#pragma warning restore 4014
        }

        [Test]
        public async Task UpdateSettings_HasLeagueUsers_PropogateDisplayName()
        {
            var league = LeagueUtility.CreateLeague(dbContext);
            var leagueUser = LeagueUtility.AddUsersToLeague(league, 1, dbContext)[0];
            const string newDisplayName = "bob";

            await testObj.UpdateSettings(leagueUser.UserID, newDisplayName, "", null);

            Assert.AreEqual(newDisplayName, leagueUser.DisplayName);
        }

        private IFormFile PrepareCdnService()
        {
            var file = Substitute.For<IFormFile>();
            cdnService.ReplaceImageAsync(Arg.Any<string>(), file, ClimbImageRules.ProfilePic).Returns(ImageKey);
            cdnService.GetImageUrl(ImageKey, ClimbImageRules.ProfilePic).Returns(ImageUrl);
            return file;
        }
    }
}