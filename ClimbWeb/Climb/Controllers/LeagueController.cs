using System;
using System.Threading.Tasks;
using Climb.Data;
using Climb.Requests.Leagues;
using Climb.Services;
using Climb.Services.ModelServices;
using Climb.ViewModels.Leagues;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Climb.Controllers
{
    public class LeagueController : BaseController<LeagueController>
    {
        private readonly ILeagueService leagueService;

        public LeagueController(ILeagueService leagueService, ApplicationDbContext dbContext, ILogger<LeagueController> logger, IUserManager userManager)
            : base(logger, userManager, dbContext)
        {
            this.leagueService = leagueService;
        }

        [HttpGet("leagues")]
        public async Task<IActionResult> Index()
        {
            var user = await GetViewUserAsync();
            var leagues = await dbContext.Leagues
                .Include(l => l.Admin).AsNoTracking()
                .Include(l => l.Members).AsNoTracking()
                .Include(l => l.Game).AsNoTracking()
                .ToArrayAsync();
            var games = await dbContext.Games.ToArrayAsync();

            var viewModel = new IndexViewModel(user, leagues, games);
            return View(viewModel);
        }

        [HttpGet("leagues/home/{leagueID:int}")]
        public async Task<IActionResult> Home(int leagueID)
        {
            var user = await GetViewUserAsync();
            var league = await dbContext.Leagues
                .Include(l => l.Game).AsNoTracking()
                .Include(l => l.Seasons).AsNoTracking()
                .Include(l => l.Members).ThenInclude(lu => lu.User).AsNoTracking()
                .FirstOrDefaultAsync(l => l.ID == leagueID);

            var viewModel = new HomeViewModel(user, league);

            return View(viewModel);
        }

        [HttpPost("leagues/create")]
        public async Task<IActionResult> Create(CreateRequest request)
        {
            try
            {
                var league = await leagueService.Create(request.Name, request.GameID, request.AdminID);
                logger.LogInformation($"League {league.ID} created.");

                return RedirectToAction("Home", new {leagueID = league.ID});
            }
            catch(Exception exception)
            {
                return GetExceptionResult(exception, request);
            }
        }

        [HttpPost("leagues/join")]
        public async Task<IActionResult> Join(JoinRequest request)
        {
            try
            {
                var leagueUser = await leagueService.Join(request.LeagueID, request.UserID);
                logger.LogInformation($"User {request.UserID} joined league {request.LeagueID} as league user {leagueUser.ID}.");

                return RedirectToAction("Home", new {leagueID = request.LeagueID});
            }
            catch(Exception exception)
            {
                return GetExceptionResult(exception, request);
            }
        }
    }
}