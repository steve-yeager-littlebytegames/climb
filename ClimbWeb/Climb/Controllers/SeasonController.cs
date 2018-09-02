using System;
using System.Net;
using System.Threading.Tasks;
using Climb.Data;
using Climb.Requests.Seasons;
using Climb.Services;
using Climb.Services.ModelServices;
using Climb.ViewModels.Seasons;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Climb.Controllers
{
    public class SeasonController : BaseController<SeasonController>
    {
        private readonly ISeasonService seasonService;
        private readonly IHostingEnvironment environment;
        private readonly IDateService dateService;

        public SeasonController(ISeasonService seasonService, ApplicationDbContext dbContext, ILogger<SeasonController> logger, IUserManager userManager, IHostingEnvironment environment, IDateService dateService)
            : base(logger, userManager, dbContext)
        {
            this.seasonService = seasonService;
            this.environment = environment;
            this.dateService = dateService;
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
                .Include(s => s.League).ThenInclude(l => l.Members).AsNoTracking()
                .FirstOrDefaultAsync(s => s.ID == seasonID);
            if(season == null)
            {
                return CodeResultAndLog(HttpStatusCode.NotFound, $"No season with ID {seasonID} found.");
            }

            var viewModel = new HomeViewModel(user, season, environment);
            return View(viewModel);
        }

        [HttpGet("seasons/membership/{seasonID:int}")]
        public async Task<IActionResult> Membership(int seasonID)
        {
            var user = await GetViewUserAsync();

            var season = await dbContext.Seasons
                .Include(s => s.Sets).ThenInclude(s => s.League).AsNoTracking()
                .Include(s => s.Sets).ThenInclude(s => s.Player1).ThenInclude(lu => lu.User).AsNoTracking()
                .Include(s => s.Sets).ThenInclude(s => s.Player2).ThenInclude(lu => lu.User).AsNoTracking()
                .Include(s => s.Participants).IgnoreQueryFilters()
                .Include(s => s.Participants).ThenInclude(slu => slu.LeagueUser).ThenInclude(lu => lu.User).AsNoTracking()
                .Include(s => s.League).ThenInclude(l => l.Members).AsNoTracking()
                .FirstOrDefaultAsync(s => s.ID == seasonID);
            if (season == null)
            {
                return CodeResultAndLog(HttpStatusCode.NotFound, $"No season with ID {seasonID} found.");
            }

            var viewModel = new MembershipViewModel(user, season, environment);
            return View(viewModel);
        }

        [HttpGet("seasons/data/{seasonID:int}")]
        public async Task<IActionResult> Data(int seasonID)
        {
            var user = await GetViewUserAsync();

            var season = await dbContext.Seasons
                .Include(s => s.Sets).ThenInclude(s => s.League).AsNoTracking()
                .Include(s => s.Sets).ThenInclude(s => s.Player1).ThenInclude(lu => lu.User).AsNoTracking()
                .Include(s => s.Sets).ThenInclude(s => s.Player2).ThenInclude(lu => lu.User).AsNoTracking()
                .Include(s => s.Participants).ThenInclude(slu => slu.LeagueUser).ThenInclude(lu => lu.User).AsNoTracking()
                .Include(s => s.League).ThenInclude(l => l.Members).AsNoTracking()
                .FirstOrDefaultAsync(s => s.ID == seasonID);
            if (season == null)
            {
                return CodeResultAndLog(HttpStatusCode.NotFound, $"No season with ID {seasonID} found.");
            }

            var viewModel = new DataViewModel(user, season, environment, dateService);
            return View(viewModel);
        }

        [HttpGet("seasons/sets/{seasonID:int}")]
        public async Task<IActionResult> Sets(int seasonID)
        {
            var user = await GetViewUserAsync();

            var season = await dbContext.Seasons
                .Include(s => s.Sets).ThenInclude(s => s.League).AsNoTracking()
                .Include(s => s.Sets).ThenInclude(s => s.Player1).ThenInclude(lu => lu.User).AsNoTracking()
                .Include(s => s.Sets).ThenInclude(s => s.Player2).ThenInclude(lu => lu.User).AsNoTracking()
                .Include(s => s.Participants).ThenInclude(slu => slu.LeagueUser).ThenInclude(lu => lu.User).AsNoTracking()
                .Include(s => s.League).ThenInclude(l => l.Members).AsNoTracking()
                .FirstOrDefaultAsync(s => s.ID == seasonID);
            if (season == null)
            {
                return CodeResultAndLog(HttpStatusCode.NotFound, $"No season with ID {seasonID} found.");
            }

            var viewModel = new SetsViewModel(user, season, environment);
            return View(viewModel);
        }

        [HttpGet("seasons/manage/{seasonID:int}")]
        public async Task<IActionResult> Manage(int seasonID)
        {
            var user = await GetViewUserAsync();

            var season = await dbContext.Seasons
                .Include(s => s.Sets).ThenInclude(s => s.League).AsNoTracking()
                .Include(s => s.Sets).ThenInclude(s => s.Player1).ThenInclude(lu => lu.User).AsNoTracking()
                .Include(s => s.Sets).ThenInclude(s => s.Player2).ThenInclude(lu => lu.User).AsNoTracking()
                .Include(s => s.Participants).ThenInclude(slu => slu.LeagueUser).ThenInclude(lu => lu.User).AsNoTracking()
                .Include(s => s.League).ThenInclude(l => l.Members).AsNoTracking()
                .FirstOrDefaultAsync(s => s.ID == seasonID);
            if (season == null)
            {
                return CodeResultAndLog(HttpStatusCode.NotFound, $"No season with ID {seasonID} found.");
            }

            var viewModel = new ManageViewModel(user, season, environment);
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

                dbContext.Update(season.League);
                season.League.ActiveSeasonID = seasonID;

                await dbContext.SaveChangesAsync();

                return RedirectToAction("Home", new {seasonID});
            }
            catch(Exception exception)
            {
                Console.WriteLine(exception);
                throw;
            }
        }

        [HttpPost("seasons/leave")]
        public async Task<IActionResult> LeavePost(int participantID)
        {
            try
            {
                var season = await seasonService.LeaveAsync(participantID);
                return RedirectToAction("Home", "League", new {leagueID = season.LeagueID});
            }
            catch(Exception exception)
            {
                return GetExceptionResult(exception, new {participantID});
            }
        }

        [HttpPost("seasons/join")]
        public async Task<IActionResult> JoinPost(string userID, int seasonID)
        {
            try
            {
                await seasonService.JoinAsync(seasonID, userID);
                return RedirectToAction("Home", new {seasonID});
            }
            catch(Exception exception)
            {
                return GetExceptionResult(exception, new {userID, seasonID});
            }
        }
    }
}