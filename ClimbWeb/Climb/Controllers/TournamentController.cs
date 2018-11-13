using System;
using System.Threading.Tasks;
using Climb.Data;
using Climb.Requests.Tournaments;
using Climb.Services;
using Climb.ViewModels.Tournaments;
using Microsoft.AspNetCore.Mvc;
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

            //var tournament = await dbContext.Tournaments.FirstOrDefaultAsync(t => t.ID == id);
            //if(tournament == null)
            //{
            //    return NotFound();
            //}

            var viewModel = new HomeViewModel(user);
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

        [HttpGet("tournaments/test/{count:int}")]
        public IActionResult Test(int count, bool randomize)
        {
            var bracketGenerator = new BracketGenerator();
            var tournament = bracketGenerator.CreateTournament(count);

            if(randomize)
            {
                Randomize();
            }

            var viewModel = new Test(tournament);
            return View(viewModel);

            void Randomize()
            {
                var random = new Random();

                for(var i = 0; i < tournament.Winners.Count; i++)
                {
                    foreach(var game in tournament.Winners[i].Games)
                    {
                        if(game.P2 == null || game.P1 != null && random.NextDouble() < 0.5)
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

                for(var i = 0; i < tournament.Losers.Count; i++)
                {
                    foreach(var game in tournament.Losers[i].Games)
                    {
                        if(game.P2 == null || game.P1 != null && random.NextDouble() < 0.5)
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