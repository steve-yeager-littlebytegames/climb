using System;
using System.Net;
using System.Threading.Tasks;
using Climb.Attributes;
using Climb.Requests.Account;
using Climb.Responses;
using Climb.Services;
using Climb.Services.ModelServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Climb.API
{
    [Route("api/v1/account")]
    public class AccountApi : BaseApi<AccountApi>
    {
        private readonly ISignInManager signInManager;
        private readonly ITokenHelper tokenHelper;
        private readonly IApplicationUserService applicationUserService;
        private readonly ICdnService cdnService;

        public AccountApi(ISignInManager signInManager, ITokenHelper tokenHelper, ILogger<AccountApi> logger, IApplicationUserService applicationUserService, ICdnService cdnService)
            : base(logger)
        {
            this.signInManager = signInManager;
            this.tokenHelper = tokenHelper;
            this.applicationUserService = applicationUserService;
            this.cdnService = cdnService;
        }

        [HttpPost("register")]
        [SwaggerResponse(HttpStatusCode.Created, typeof(UserDto))]
        [SwaggerResponse(HttpStatusCode.BadRequest, typeof(string), "Email or password is not valid.")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            try
            {
                var user = await applicationUserService.Register(request, Url, Request.Scheme);
                var dto = UserDto.Create(user, cdnService);
                return GetCodeResult(HttpStatusCode.Created, dto, "Created user.");
            }
            catch(Exception exception)
            {
                return GetExceptionResult(exception, request);
            }
        }

        [HttpPost("logIn")]
        [AllowAnonymous]
        [SwaggerResponse(HttpStatusCode.OK, typeof(string))]
        [Attributes.SwaggerResponse(HttpStatusCode.BadRequest, typeof(string), "Email or password is incorrect.")]
        public async Task<IActionResult> LogIn(LoginRequest request)
        {
            try
            {
                var token = await applicationUserService.LogIn(request);
                return GetCodeResult(HttpStatusCode.OK, token, "User logged in.");
            }
            catch(Exception exception)
            {
                return GetExceptionResult(exception, request);
            }
        }

        [HttpPost("logOut")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(string))]
        public async Task<IActionResult> Logout([UserToken] string auth)
        {
            await signInManager.SignOutAsync();
            logger.LogInformation("User logged out.");
            return RedirectToPage("/Index");
        }

        [HttpGet("test")]
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
    }
}