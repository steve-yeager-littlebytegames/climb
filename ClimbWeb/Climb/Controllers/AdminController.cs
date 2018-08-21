using System;
using System.Threading.Tasks;
using Climb.Data;
using Climb.Services.ModelServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Climb.Controllers
{
    public class AdminController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly ApplicationDbContext dbContext;
        private readonly IServiceProvider serviceProvider;
        private readonly ILogger<AdminController> logger;

        public AdminController(IConfiguration configuration, ApplicationDbContext dbContext, IServiceProvider serviceProvider, ILogger<AdminController> logger)
        {
            this.configuration = configuration;
            this.dbContext = dbContext;
            this.serviceProvider = serviceProvider;
            this.logger = logger;
        }

        [HttpPost("admin/data/migrate")]
        public async Task<IActionResult> Migrate([FromHeader]string key)
        {
            if(!Validate(key))
            {
                return Unauthorized();
            }

            try
            {
                var userManager = serviceProvider.GetService<UserManager<ApplicationUser>>();
                await DataMigrator.MigrateV1(dbContext, userManager, configuration.GetConnectionString("ClimbV1"));
                return Ok();
            }
            catch(Exception exception)
            {
                logger.LogError("Failed migrating data.", exception);
                return StatusCode(500, exception);
            }
        }

        [HttpPost("admin/update-all-leagues")]
        public async Task<IActionResult> UpdateAllLeagues([FromHeader] string key)
        {
            if(!Validate(key))
            {
                return Unauthorized();
            }

            var leagueService = serviceProvider.GetService<ILeagueService>();

            try
            {
                var leagues = await dbContext.Leagues.ToListAsync();
                foreach(var league in leagues)
                {
                    await leagueService.UpdateStandings(league.ID);
                    await leagueService.TakeSnapshots(league.ID);
                }

                return Ok();
            }
            catch(Exception exception)
            {
                logger.LogError("Failed updating all leagues.", exception);
                return StatusCode(500, exception);
            }
        }

        private bool Validate(string key)
        {
            var adminKey = configuration["AdminKey"];
            return adminKey == key;
        }
    }
}