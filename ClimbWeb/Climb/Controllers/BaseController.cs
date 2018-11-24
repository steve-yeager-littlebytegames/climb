using System;
using System.Net;
using System.Threading.Tasks;
using Climb.Data;
using Climb.Exceptions;
using Climb.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;

namespace Climb.Controllers
{
    [SwaggerIgnore]
    public abstract class BaseController<T> : Controller where T : Controller
    {
        protected readonly ILogger<T> logger;
        protected readonly IUserManager userManager;
        protected readonly ApplicationDbContext dbContext;

        protected string BaseUrl => $"{Request.Scheme}://{Request.Host}";

        protected BaseController(ILogger<T> logger, IUserManager userManager, ApplicationDbContext dbContext)
        {
            this.logger = logger;
            this.userManager = userManager;
            this.dbContext = dbContext;
        }

        protected async Task<ApplicationUser> GetViewUserAsync()
        {
            var appUser = await userManager.GetUserAsync(User);
            if(appUser == null)
            {
                return null;
            }

            var loadedUser = await dbContext.Users
                .Include(u => u.LeagueUsers).ThenInclude(lu => lu.League).ThenInclude(l => l.Game).AsNoTracking()
                .Include(u => u.Seasons).ThenInclude(slu => slu.Season).ThenInclude(s => s.League).AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == appUser.Id);

            return loadedUser;
        }

        protected IActionResult GetExceptionResult(Exception exception, object request)
        {
            switch(exception)
            {
                case NotFoundException _: return GetCodeResult(HttpStatusCode.NotFound, exception.Message);
                case BadRequestException _: return GetCodeResult(HttpStatusCode.BadRequest, exception.Message);
                case ConflictException _: return GetCodeResult(HttpStatusCode.Conflict, exception.Message);
                case NotAuthorizedException _: return GetCodeResult(HttpStatusCode.Forbidden, exception.Message);
                default:
                    logger.LogError(exception, $"Error handling request\n{request}");
                    return GetCodeResult(HttpStatusCode.InternalServerError, "Server Error");
            }
        }

        protected ObjectResult GetCodeResult(HttpStatusCode code, string value)
        {
            logger.LogInformation(value);
            return new ObjectResult(value) {StatusCode = (int)code};
        }
    }
}