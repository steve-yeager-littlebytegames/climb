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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Climb.Controllers
{
    public class SeasonController : BaseController<SeasonController>
    {
        private readonly ISeasonService seasonService;
        private readonly IDateService dateService;
        private readonly ICdnService cdnService;
        private readonly IConfiguration configuration;

        public SeasonController(ISeasonService seasonService, ApplicationDbContext dbContext, ILogger<SeasonController> logger, IUserManager userManager, IDateService dateService, ICdnService cdnService, IConfiguration configuration)
            : base(logger, userManager, dbContext)
        {
            this.seasonService = seasonService;
            this.dateService = dateService;
            this.cdnService = cdnService;
            this.configuration = configuration;
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
                return GetCodeResult(HttpStatusCode.NotFound, $"No season with ID {seasonID} found.");
            }

            var viewModel = new HomeViewModel(user, season, configuration);
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
                return GetCodeResult(HttpStatusCode.NotFound, $"No season with ID {seasonID} found.");
            }

            var viewModel = new MembershipViewModel(user, season, configuration);
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
                return GetCodeResult(HttpStatusCode.NotFound, $"No season with ID {seasonID} found.");
            }

            var viewModel = new DataViewModel(user, season, configuration, dateService);
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
                return GetCodeResult(HttpStatusCode.NotFound, $"No season with ID {seasonID} found.");
            }

            var viewModel = new SetsViewModel(user, season, configuration);
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
                return GetCodeResult(HttpStatusCode.NotFound, $"No season with ID {seasonID} found.");
            }

            var viewModel = new ManageViewModel(user, season, configuration);
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

        [HttpGet("season/details/{participantID:int}")]
        public async Task<IActionResult> Details(int participantID)
        {
            var user = await GetViewUserAsync();

            var participant = await dbContext.SeasonLeagueUsers
                .Include(slu => slu.User).AsNoTracking()
                .Include(slu => slu.Season).ThenInclude(s => s.League).ThenInclude(l => l.Members).IgnoreQueryFilters().AsNoTracking()
                .Include(slu => slu.Season).ThenInclude(s => s.Participants).IgnoreQueryFilters().AsNoTracking()
                .Include(slu => slu.LeagueUser).AsNoTracking()
                .Include(slu => slu.P1Sets).ThenInclude(s => s.SeasonPlayer2).ThenInclude(slu => slu.LeagueUser).AsNoTracking()
                .Include(slu => slu.P2Sets).ThenInclude(s => s.SeasonPlayer1).ThenInclude(slu => slu.LeagueUser).AsNoTracking()
                .FirstOrDefaultAsync(slu => slu.ID == participantID);
            if(participant == null)
            {
                return NotFound();
            }

            var viewModel = new DetailsViewModel(user, participant, participant.Season, configuration, cdnService);
            return View(viewModel);
        }
    }
}