using System;
using System.Net;
using System.Threading.Tasks;
using Climb.Attributes;
using Climb.Data;
using Climb.Requests.Account;
using Climb.Services;
using Climb.Services.ModelServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Climb.API
{
    public class AccountApi : BaseApi<AccountApi>
    {
        private readonly ISignInManager signInManager;
        private readonly ITokenHelper tokenHelper;
        private readonly IApplicationUserService applicationUserService;

        public AccountApi(ISignInManager signInManager, ITokenHelper tokenHelper, ILogger<AccountApi> logger, IApplicationUserService applicationUserService)
            : base(logger)
        {
            this.signInManager = signInManager;
            this.tokenHelper = tokenHelper;
            this.applicationUserService = applicationUserService;
        }

        [HttpPost("/api/v1/account/register")]
        [SwaggerResponse(HttpStatusCode.Created, typeof(ApplicationUser))]
        [SwaggerResponse(HttpStatusCode.BadRequest, typeof(string), "Email or password is not valid.")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            try
            {
                var user = await applicationUserService.Register(request, Url, Request.Scheme);
                return CodeResultAndLog(HttpStatusCode.Created, user, "Created user.");
            }
            catch(Exception exception)
            {
                return GetExceptionResult(exception, request);
            }
        }

        [HttpPost("/api/v1/account/logIn")]
        [AllowAnonymous]
        [SwaggerResponse(HttpStatusCode.OK, typeof(string))]
        [SwaggerResponse(HttpStatusCode.BadRequest, typeof(string), "Email or password is incorrect.")]
        public async Task<IActionResult> LogIn(LoginRequest request)
        {
            try
            {
                var token = await applicationUserService.LogIn(request);
                return CodeResultAndLog(HttpStatusCode.OK, token, "User logged in.");
            }
            catch(Exception exception)
            {
                return GetExceptionResult(exception, request);
            }
        }

        [HttpGet("/api/v1/account/test")]
        [Authorize]
        [SwaggerResponse(HttpStatusCode.OK, typeof(string))]
        public async Task<IActionResult> Test([UserToken] string auth, string userID)
        {
            var authorizedId = await tokenHelper.GetAuthorizedUserID(auth);

            if(userID == authorizedId || string.IsNullOrWhiteSpace(userID))
            {
                return Ok("Authorized!");
            }

            return BadRequest("Not the same user!");
        }

        [HttpPost("/api/v1/account/logOut")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(string))]
        public async Task<IActionResult> Logout([UserToken] string auth)
        {
            await signInManager.SignOutAsync();
            logger.LogInformation("User logged out.");
            return RedirectToPage("/Index");
        }
    }
}