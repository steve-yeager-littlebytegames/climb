using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Climb.Data;
using Climb.Models;
using Climb.Requests.Tournaments;
using Climb.Services;
using Climb.ViewModels.Tournaments;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Climb.Controllers
{
    public class TournamentController : BaseController<TournamentController>
    {
        private readonly ITournamentService tournamentService;
        private readonly IConfiguration configuration;

        public TournamentController(ILogger<TournamentController> logger, IUserManager userManager, ApplicationDbContext dbContext, ITournamentService tournamentService, IConfiguration configuration)
            : base(logger, userManager, dbContext)
        {
            this.tournamentService = tournamentService;
            this.configuration = configuration;
        }

        [HttpGet("tournaments/home/{id:int}")]
        public async Task<IActionResult> Home(int id)
        {
            var user = await GetViewUserAsync();

            var tournament = await dbContext.Tournaments
                .Include(t => t.League).AsNoTracking()
                .Include(t => t.Season).AsNoTracking()
                .Include(t => t.Rounds).AsNoTracking()
                .Include(t => t.Sets).AsNoTracking()
                .Include(t => t.SetSlots).ThenInclude(ss => ss.Round).AsNoTracking()
                .Include(t => t.SetSlots).ThenInclude(ss => ss.User1).ThenInclude(tu => tu.LeagueUser).AsNoTracking()
                .Include(t => t.SetSlots).ThenInclude(ss => ss.User2).ThenInclude(tu => tu.LeagueUser).AsNoTracking()
                .Include(t => t.TournamentUsers).ThenInclude(tu => tu.LeagueUser).AsNoTracking()
                .FirstOrDefaultAsync(t => t.ID == id);
            if (tournament == null)
            {
                return NotFound();
            }

            var viewModel = new HomeViewModel(user, tournament, configuration);
            return View(viewModel);
        }

        [HttpGet("tournaments/sets/{id:int}")]
        public async Task<IActionResult> Sets(int id)
        {
            var user = await GetViewUserAsync();

            var tournament = await dbContext.Tournaments
                .Include(t => t.Sets).ThenInclude(s => s.League).AsNoTracking()
                .Include(t => t.Sets).ThenInclude(s => s.Player1).ThenInclude(lu => lu.User).AsNoTracking()
                .Include(t => t.Sets).ThenInclude(s => s.Player2).ThenInclude(lu => lu.User).AsNoTracking()
                .FirstOrDefaultAsync(t => t.ID == id);
            if (tournament == null)
            {
                return NotFound();
            }

            var viewModel = new SetsViewModel(tournament.Sets, tournament, user, configuration);
            return View(viewModel);
        }

        [HttpGet("tournaments/competitors/{id:int}")]
        public async Task<IActionResult> Competitors(int id)
        {
            var user = await GetViewUserAsync();

            var tournament = await dbContext.Tournaments
                .Include(t => t.TournamentUsers).ThenInclude(tu => tu.LeagueUser).AsNoTracking()
                .FirstOrDefaultAsync(t => t.ID == id);

            var viewModel = new CompetitorsViewModel(user, tournament, configuration);
            return View(viewModel);
        }
        
        [HttpGet("tournaments/manage/{id:int}")]
        public async Task<IActionResult> Manage(int id)
        {
            var user = await GetViewUserAsync();

            var tournament = await dbContext.Tournaments
                .Include(t => t.TournamentUsers).ThenInclude(tu => tu.LeagueUser).AsNoTracking()
                .FirstOrDefaultAsync(t => t.ID == id);

            var viewModel = new ManageViewModel(user, tournament, configuration);
            return View(viewModel);
        }

        [HttpPost("tournaments")]
        public async Task<IActionResult> Create(CreateRequest request)
        {
            try
            {
                var tournament = await tournamentService.Create(request.LeagueID, request.SeasonID, request.Name);
                return RedirectToAction("Home", new
                {
                    tournament.ID
                });
            }
            catch(Exception exception)
            {
                return GetExceptionResult(exception, request);
            }
        }

        //[HttpGet("tournaments/test/{count:int}")]
        //public async Task<IActionResult> Test(int count, bool randomize)
        //{
        //    var league = await dbContext.Leagues.FirstOrDefaultAsync();

        //    var bracketGenerator = new BracketGenerator();
        //    var tournamentData = bracketGenerator.Generate(count);
        //    var tournament = new Tournament(league.ID, DateTime.Now)
        //    {
        //        Rounds = new List<Round>(),
        //    };
        //    tournamentService.AddBracket(tournament, tournamentData);

        //    return View("Home", new HomeViewModel(null, tournament, configuration));
        //}
    }
}