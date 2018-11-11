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

            Randomize();

            var viewModel = new Test(tournament);
            return View(viewModel);

            void Randomize()
            {
                var random = new Random();

                for(int i = 0; i < tournament.Winners.Rounds.Count - 1; i++)
                {
                    foreach(var game in tournament.Winners.Rounds[i].Games)
                    {
                        if(random.NextDouble() < 0.5)
                        {
                            game.NextWin.AddPlayer(game.P1);
                            game.NextLoss.AddPlayer(game.P2);
                            game.P1Score = 2;
                            game.P2Score = random.Next(0, 2);
                        }
                        else
                        {
                            game.NextWin.AddPlayer(game.P2);
                            game.NextLoss.AddPlayer(game.P1);
                            game.P1Score = random.Next(0, 2);
                            game.P2Score = 2;
                        }
                    }
                }

                for(int i = 0; i < tournament.Losers.Rounds.Count - 1; i++)
                {
                    foreach(var game in tournament.Losers.Rounds[i].Games)
                    {
                        if(random.NextDouble() < 0.5)
                        {
                            game.NextWin.AddPlayer(game.P1);
                            game.P1Score = 2;
                            game.P2Score = random.Next(0, 2);
                        }
                        else
                        {
                            game.NextWin.AddPlayer(game.P2);
                            game.P1Score = random.Next(0, 2);
                            game.P2Score = 2;
                        }
                    }
                }
            }
        }
    }
}