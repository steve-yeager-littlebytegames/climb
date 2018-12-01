using System;
using System.Net;
using Climb.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Climb.API
{
    public abstract class BaseApi<T> : Controller where T : Controller
    {
        protected readonly ILogger<T> logger;

        protected BaseApi(ILogger<T> logger)
        {
            this.logger = logger;
        }

        protected IActionResult GetExceptionResult(Exception exception, object request)
        {
            switch(exception)
            {
                case NotFoundException _: return GetCodeResult(HttpStatusCode.NotFound, exception.Message);
                case BadRequestException _: return GetCodeResult(HttpStatusCode.BadRequest, exception.Message);
                case ConflictException _: return GetCodeResult(HttpStatusCode.Conflict, exception.Message);
                default:
                    logger.LogError(exception, $"Error handling request\n{request}");
                    return CodeResult(HttpStatusCode.InternalServerError, "Server Error");
            }
        }

        protected ObjectResult CodeResult(HttpStatusCode code, object value)
        {
            return new ObjectResult(value) {StatusCode = (int)code};
        }

        protected ObjectResult GetCodeResult(HttpStatusCode code, object value, string message)
        {
            logger.LogInformation(message);
            return new ObjectResult(value) {StatusCode = (int)code};
        }

        protected ObjectResult GetCodeResult(HttpStatusCode code, string value)
        {
            logger.LogInformation(value);
            return new ObjectResult(value) {StatusCode = (int)code};
        }
    }
}