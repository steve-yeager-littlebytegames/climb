using System;
using System.Net;
using System.Threading.Tasks;
using Climb.Data;
using Climb.Requests.Seasons;
using Climb.Services;
using Climb.Services.ModelServices;
using Climb.ViewModels.Seasons;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Climb.Controllers
{
    public class SeasonController : BaseController<SeasonController>
    {
        private readonly ISeasonService seasonService;

        public SeasonController(ISeasonService seasonService, ApplicationDbContext dbContext, ILogger<SeasonController> logger, IUserManager userManager)
            : base(logger, userManager, dbContext)
        {
            this.seasonService = seasonService;
        }

        [HttpGet("seasons/home/{seasonID:int}")]
        public async Task<IActionResult> Home(int seasonID)
        {
            var user = await GetViewUserAsync();

            var season = await dbContext.Seasons
                .Include(s => s.Sets).ThenInclude(s => s.League).AsNoTracking()
                .Include(s => s.Sets).ThenInclude(s => s.Player1).ThenInclude(lu => lu.User).AsNoTracking()
                .Include(s => s.Sets).ThenInclude(s => s.Player2).ThenInclude(lu => lu.User).AsNoTracking()
                .Include(s => s.Participants).ThenInclude(slu => slu.LeagueUser).ThenInclude(lu => lu.User).AsNoTracking()
                .Include(s => s.League).AsNoTracking()
                .FirstOrDefaultAsync(s => s.ID == seasonID);
            if(season == null)
            {
                return CodeResultAndLog(HttpStatusCode.NotFound, $"No season with ID {seasonID} found.");
            }

            var viewModel = HomeViewModel.Create(user, season);
            return View(viewModel);
        }
        
        [HttpPost("seasons/create")]
        public async Task<IActionResult> Create(CreateRequest request)
        {
            try
            {
                var season = await seasonService.Create(request.LeagueID, request.StartDate, request.EndDate);
                logger.LogInformation($"Season {season.ID} created for League {season.LeagueID}.");
                return RedirectToAction("Home", new {seasonID = season.ID});
            }
            catch(Exception exception)
            {
                Console.WriteLine(exception);
                throw;
            }
        }

        [HttpPost("seasons/start")]
        public async Task<IActionResult> Start(int seasonID)
        {
            try
            {
                var season = await seasonService.GenerateSchedule(seasonID);
                dbContext.Update(season);
                season.IsActive = true;
                await dbContext.SaveChangesAsync();

                return RedirectToAction("Home", new {seasonID});
            }
            catch(Exception exception)
            {
                Console.WriteLine(exception);
                throw;
            }
        }
    }
}