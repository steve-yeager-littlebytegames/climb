using System;
using System.Threading.Tasks;
using Climb.Data;
using Climb.Services;
using Climb.ViewModels;
using Climb.ViewModels.Site;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Climb.Controllers
{
    public class SiteController : BaseController<SiteController>
    {
        private readonly IEmailSender emailSender;
        private readonly IConfiguration configuration;

        public SiteController(ILogger<SiteController> logger, IUserManager userManager, ApplicationDbContext dbContext, IEmailSender emailSender, IConfiguration configuration)
            : base(logger, userManager, dbContext)
        {
            this.emailSender = emailSender;
            this.configuration = configuration;
        }

        [HttpGet("")]
        public async Task<IActionResult> Home()
        {
            var user = await GetViewUserAsync();

            var viewModel = await HomeViewModel.Create(user, dbContext);
            return View(viewModel);
        }

        public IActionResult Error(int? statusCode = null)
        {
            var feature = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
            ViewData["ErrorUrl"] = feature?.OriginalPath;
            ViewData["ErrorQuerystring"] = feature?.OriginalQueryString;

            if(statusCode.HasValue)
            {
                if(statusCode == 404 || statusCode == 500)
                {
                    var viewName = statusCode.ToString();
                    return View($"Error{viewName}");
                }
            }

            return View("Error500");
        }

        [HttpGet("Support")]
        public async Task<IActionResult> Support()
        {
            var user = await GetViewUserAsync();

            if(TempData.TryGetValue("success", out var success))
            {
                ViewData["Success"] = success;
            }

            var viewModel = new BaseViewModel(user);
            return View(viewModel);
        }

        [Authorize]
        [HttpPost("SendSupportTicket")]
        public async Task<IActionResult> SendSupportTicket(string summary, string description)
        {
            var supportEmail = configuration.GetSection("Email")["Support"];

            var user = await userManager.GetUserAsync(User);
            var message = $"<b>From:</b> {user.Email}<br/><br/>"
                          + $"<b>Summary</b><br/>{summary}<br/><br/>"
                          + $"<b>Description</b><br/>{description}";

            try
            {
                await emailSender.SendEmailAsync(supportEmail, "Support Ticket", message);
                TempData["success"] = true;
            }
            catch(Exception)
            {
                TempData["success"] = false;
            }

            return RedirectToAction("Support");
        }
    }
}