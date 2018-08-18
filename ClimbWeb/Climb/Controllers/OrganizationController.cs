using System;
using System.Threading.Tasks;
using Climb.Data;
using Climb.Models;
using Climb.Services;
using Climb.Services.ModelServices;
using Climb.ViewModels.Organizations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Climb.Controllers
{
    public class OrganizationController : BaseController<OrganizationController>
    {
        private readonly IOrganizationService organizationService;

        public OrganizationController(ILogger<OrganizationController> logger, IUserManager userManager, ApplicationDbContext dbContext, IOrganizationService organizationService)
            : base(logger, userManager, dbContext)
        {
            this.organizationService = organizationService;
        }

        [HttpGet("organizations")]
        public async Task<IActionResult> Index()
        {
            var user = await GetViewUserAsync();
            var organizations = await dbContext.Organizations
                .Include(l => l.Members).AsNoTracking()
                .Include(l => l.Leagues).ThenInclude(l => l.Members).AsNoTracking()
                .ToArrayAsync();

            var viewModel = new IndexViewModel(user, organizations);
            return View(viewModel);
        }

        [HttpGet("organizations/home/{organizationID:int}")]
        public async Task<IActionResult> Home(int organizationID)
        {
            var user = await GetViewUserAsync();
            var organization = await dbContext.Organizations
                .Include(l => l.Leagues).ThenInclude(l => l.Members).AsNoTracking()
                .Include(l => l.Members).AsNoTracking()
                .FirstOrDefaultAsync(l => l.ID == organizationID);

            var viewModel = new HomeViewModel(user, organization);

            return View(viewModel);
        }

        // TODO: Move to service.
        [Authorize]
        [HttpPost("organizations/update")]
        public async Task<IActionResult> UpdatePost(int? id, string name)
        {
            var user = await GetViewUserAsync();

            var organization = new Organization
            {
                Name = name,
                DateCreated = DateTime.Now,
            };
            dbContext.Organizations.Add(organization);

            var owner = new OrganizationUser
            {
                OrganizationID = organization.ID,
                UserID = user.Id,
                IsOwner = true,
            };
            dbContext.OrganizationUsers.Add(owner);

            await dbContext.SaveChangesAsync();

            return RedirectToAction("Home", new {organizationID = organization.ID});
        }

        [HttpPost("Organizations/AddLeague")]
        public async Task<IActionResult> AddLeaguePost(int organizationID, int leagueID)
        {
            try
            {
                var user = await GetViewUserAsync();

                await organizationService.AddLeague(organizationID, leagueID, user.Id);
                return RedirectToAction("Home", new {organizationID});
            }
            catch(Exception exception)
            {
                return GetExceptionResult(exception, new {organizationID, leagueID});
            }
        }
    }
}