using System;
using System.Net;
using System.Threading.Tasks;
using Climb.Attributes;
using Climb.Data;
using Climb.Services;
using Climb.Services.ModelServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Climb.API
{
    public class AdminApi : BaseApi<AdminApi>
    {
        private readonly IConfiguration configuration;
        private readonly ApplicationDbContext dbContext;
        private readonly IServiceProvider serviceProvider;
        private readonly IDateService dateService;

        public AdminApi(ILogger<AdminApi> logger, IConfiguration configuration, ApplicationDbContext dbContext, IServiceProvider serviceProvider, IDateService dateService)
            : base(logger)
        {
            this.configuration = configuration;
            this.dbContext = dbContext;
            this.serviceProvider = serviceProvider;
            this.dateService = dateService;
        }

        [HttpPost("admin/update-all-leagues")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(string))]
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
                    var hasSets = await dbContext.Sets.AnyAsync(s => s.LeagueID == league.ID && s.UpdatedDate >= league.LastRankUpdate);

                    if(!hasSets)
                    {
                        logger.LogInformation($"League {league} hasn't had any sets played.");
                        continue;
                    }

                    logger.LogInformation($"Updating PR for League {league}.");

                    await leagueService.UpdateStandings(league.ID);
                    await leagueService.TakeSnapshots(league.ID);

                    dbContext.Update(league);
                    league.LastRankUpdate = dateService.Now;
                    await dbContext.SaveChangesAsync();

                    logger.LogInformation($"Done updating PR for League {league}.");
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