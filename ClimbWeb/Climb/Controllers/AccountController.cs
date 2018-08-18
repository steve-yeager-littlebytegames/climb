using System;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Climb.Data;
using Climb.Requests.Account;
using Climb.Services;
using Climb.Services.ModelServices;
using Climb.ViewModels;
using Climb.ViewModels.Account;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Climb.Controllers
{
    public class AccountController : BaseController<AccountController>
    {
        public const string EmailKey = "Email";
        private const string LoginFail = "LoginFail";
        private readonly IApplicationUserService applicationUserService;
        private readonly ISignInManager signInManager;
        private readonly ICdnService cdnService;
        private readonly IEmailSender emailSender;

        public AccountController(ILogger<AccountController> logger, IApplicationUserService applicationUserService, IUserManager userManager, ApplicationDbContext dbContext, ISignInManager signInManager, ICdnService cdnService, IEmailSender emailSender)
            : base(logger, userManager, dbContext)
        {
            this.applicationUserService = applicationUserService;
            this.signInManager = signInManager;
            this.cdnService = cdnService;
            this.emailSender = emailSender;
        }

        [HttpGet("account/register")]
        public async Task<IActionResult> Register()
        {
            var user = await GetViewUserAsync();
            if(user != null)
            {
                return RedirectToAction("Home", "User", new {userID = user.Id});
            }

            var viewModel = new RegisterViewModel(null, new RegisterRequest());
            return View(viewModel);
        }

        [HttpPost("account/register")]
        public async Task<IActionResult> RegisterPost(RegisterRequest request)
        {
            if(!ModelState.IsValid)
            {
                var viewModel = new RegisterViewModel(null, request);
                return View("Register", viewModel);
            }

            try
            {
                await applicationUserService.Register(request, Url, Request.Scheme);

                return RedirectToAction("Home", "User");
            }
            catch(Exception exception)
            {
                logger.LogError(exception, $"Error handling request\n{request}");
                return RedirectToAction("Register");
            }
        }

        [HttpGet("account/login")]
        public async Task<IActionResult> LogIn()
        {
            if(TempData.ContainsKey(LoginFail))
            {
                TempData.Remove(LoginFail);
                ViewData[LoginFail] = true;
            }

            var user = await GetViewUserAsync();
            if(user != null)
            {
                return RedirectToAction("Home", "User", new {userID = user.Id});
            }

            var viewModel = new BaseViewModel(null);
            return View(viewModel);
        }

        [HttpPost("account/login")]
        public async Task<IActionResult> LogInPost(LoginRequest request)
        {
            try
            {
                await applicationUserService.LogIn(request);

                return RedirectToAction("Home", "User");
            }
            catch(Exception exception)
            {
                logger.LogError(exception, "Failed to log in.");
                TempData[LoginFail] = true;
                return RedirectToAction("LogIn");
            }
        }

        [HttpPost("account/logout")]
        public async Task<IActionResult> LogOut()
        {
            await signInManager.SignOutAsync();

            return RedirectToAction("Home", "Site");
        }

        [HttpGet("account/settings")]
        public async Task<IActionResult> Settings()
        {
            var user = await GetViewUserAsync();

            var viewModel = SettingsViewModel.Create(user, cdnService);
            return View(viewModel);
        }

        [HttpPost("account/updatesettings")]
        public async Task<IActionResult> UpdateSettings(UpdateSettingsRequest request)
        {
            var user = await GetViewUserAsync();

            // TODO: Handle errors.
            await applicationUserService.UpdateSettings(user.Id, request.Username, request.Name, request.ProfilePic);

            return RedirectToAction("Settings");
        }

        [HttpGet("account/forgotpassword")]
        public async Task<IActionResult> ForgotPassword()
        {
            var user = await GetViewUserAsync();

            if(user != null)
            {
                return RedirectToAction("Home", "User", new {userID = user.Id});
            }

            if(TempData.ContainsKey(EmailKey))
            {
                ViewData[EmailKey] = TempData[EmailKey];
            }

            var viewModel = new BaseViewModel(null);
            return View(viewModel);
        }

        [HttpPost("account/forgotpassword")]
        public async Task<IActionResult> ForgotPasswordPost(string email)
        {
            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
            if(user != null)
            {
                var token = await userManager.GeneratePasswordResetTokenAsync(user);
                var resetUrl = Url.Action("ResetPassword", "Account", new {userID=user.Id, code=token}, Url.ActionContext.HttpContext.Request.Scheme);
                resetUrl = HtmlEncoder.Default.Encode(resetUrl);

                var builder = new StringBuilder();
                builder.AppendLine("<p>Looks like you forgot your Climb password! Here is a link to reset it. Now get back in the fight!</p>");
                builder.AppendLine($"<a href='{resetUrl}'>Reset Password</a>");

                await emailSender.SendEmailAsync(email, "Password Reset", builder.ToString());
            }

            TempData[EmailKey] = email;

            return RedirectToAction("ForgotPassword");
        }

        [HttpGet("account/resetpassword")]
        public IActionResult ResetPassword(string userID, string code)
        {
            ViewData["User"] = userID;
            ViewData["Code"] = code;
            var viewModel = new BaseViewModel(null);
            return View(viewModel);
        }

        [HttpPost("account/resetpassword")]
        public async Task<IActionResult> ResetPasswordPost(string userID, string code, string password, string confirm)
        {
            if(password != confirm)
            {
                return BadRequest("Passwords do not match.");
            }

            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == userID);
            if(user == null)
            {
                return BadRequest("No user found.");
            }

            var resetResult = await userManager.ResetPasswordAsync(user, code, password);
            if(!resetResult.Succeeded)
            {
                return BadRequest(string.Join('\n', resetResult.Errors.Select(e => $"{e.Code}:{e.Description}")));
            }

            return RedirectToAction("LogIn");
        }
    }
}