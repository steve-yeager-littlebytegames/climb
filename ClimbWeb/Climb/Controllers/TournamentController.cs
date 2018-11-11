using System;
using System.Threading.Tasks;
using Climb.Data;
using Climb.Models;
using Climb.Requests.Tournaments;
using Climb.Services;
using Climb.ViewModels;
using Climb.ViewModels.Tournaments;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Climb.Controllers
{
    public class TournamentController : BaseController<TournamentController>
    {
        private readonly ITournamentService tournamentService;

        public TournamentController(ILogger<TournamentController> logger, IUserManager userManager, ApplicationDbContext dbContext, ITournamentService tournamentService)
            : base(logger, userManager, dbContext)
        {
            this.tournamentService = tournamentService;
        }

        [HttpGet("tournaments/home/{id:int}")]
        public async Task<IActionResult> Home(int id)
        {
            var user = await GetViewUserAsync();

            var tournament = await dbContext.Tournaments.FirstOrDefaultAsync(t => t.ID == id);
            if(tournament == null)
            {
                return NotFound();
            }

            var viewModel = new GenericViewModel<Tournament>(user, tournament);
            return View(viewModel);
        }

        [HttpPost("tournaments")]
        public async Task<IActionResult> Create(CreateRequest request)
        {
            try
            {
                var tournament = await tournamentService.Create(request.LeagueID, request.SeasonID, request.Name);
                return RedirectToAction("Home", new {tournament.ID});
            }
            catch(Exception exception)
            {
                return GetExceptionResult(exception, request);
            }
        }

        [HttpGet("tournaments/test/{count:int}")]
        public IActionResult Test(int count)
        {
            var bracketGenerator = new BracketGenerator();
            var tournament = bracketGenerator.CreateTournament(count);

            var viewModel = new Test(tournament);
            return View(viewModel);
        }
    }
}