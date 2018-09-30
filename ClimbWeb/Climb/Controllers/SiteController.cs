using System;
using System.Linq;
using System.Threading.Tasks;
using Climb.Data;
using Climb.Services;
using Climb.ViewModels;
using Climb.ViewModels.Site;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        [HttpGet("Search")]
        public async Task<IActionResult> Search([CanBeNull] [FromQuery] string search)
        {
            var user = await GetViewUserAsync();

            if(string.IsNullOrWhiteSpace(search))
            {
                return View(new SearchViewModel(user));
            }

            var normalizedSearch = search.ToUpperInvariant();

            var gameResults = await dbContext.Games
                .Where(g => g.Name.ToUpperInvariant().Contains(normalizedSearch))
                .ToArrayAsync();
            var leagueResults = await dbContext.Leagues
                .Include(l => l.Game)
                .Where(l => l.Name.ToUpperInvariant().Contains(normalizedSearch)
                            || l.Game.Name.ToUpperInvariant().Contains(normalizedSearch))
                .ToArrayAsync();
            var userResults = await dbContext.Users
                .Where(u => u.NormalizedUserName.Contains(normalizedSearch)
                            || (!string.IsNullOrWhiteSpace(u.Name) && u.Name.ToUpperInvariant().Contains(normalizedSearch)))
                .ToArrayAsync();

            var viewModel = new SearchViewModel(user, search, gameResults, leagueResults, userResults);
            return View(viewModel);
        }

        [HttpGet("About")]
        public async Task<IActionResult> About()
        {
            var user = await GetViewUserAsync();
            var viewModel = new AboutViewModel(user, configuration, BaseUrl);
            return View(viewModel);
        }
    }
}