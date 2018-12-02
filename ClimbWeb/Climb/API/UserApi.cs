using System;
using System.Net;
using System.Threading.Tasks;
using Climb.Attributes;
using Climb.Data;
using Climb.Responses;
using Climb.Services;
using Climb.Services.ModelServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Climb.API
{
    [Route("api/v1/users")]
    public class UserApi : BaseApi<UserApi>
    {
        private readonly ApplicationDbContext dbContext;
        private readonly ICdnService cdnService;
        private readonly IApplicationUserService applicationUserService;

        public UserApi(ILogger<UserApi> logger, ApplicationDbContext dbContext, ICdnService cdnService, IApplicationUserService applicationUserService)
            : base(logger)
        {
            this.dbContext = dbContext;
            this.cdnService = cdnService;
            this.applicationUserService = applicationUserService;
        }

        [HttpGet("{id:int}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(UserDto))]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(string), "Could not find user.")]
        public async Task<IActionResult> Get(string id)
        {
            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == id);
            if(user == null)
            {
                return GetCodeResult(HttpStatusCode.NotFound, $"Could not find user with ID '{id}'.");
            }

            var response = UserDto.Create(user, cdnService);
            return CodeResult(HttpStatusCode.OK, response);
        }

        [HttpPost("{id:int}/uploadProfilePic")]
        [SwaggerResponse(HttpStatusCode.Created, typeof(string), "Profile picture URL.")]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(string), "Couldn't find user.")]
        public async Task<IActionResult> UploadProfilePic(string id, IFormFile image)
        {
            try
            {
                var imageUrl = await applicationUserService.UploadProfilePic(id, image);
                return GetCodeResult(HttpStatusCode.Created, imageUrl, $"Uploaded new profile pic for {id}.");
            }
            catch(Exception exception)
            {
                return GetExceptionResult(exception, new {id, image});
            }
        }
    }
}