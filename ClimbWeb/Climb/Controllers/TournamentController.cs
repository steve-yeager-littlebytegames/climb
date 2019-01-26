﻿using System;
using System.Threading.Tasks;
using Climb.Data;
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
                .Include(t => t.TournamentUsers).ThenInclude(tu => tu.LeagueUser).AsNoTracking()
                .FirstOrDefaultAsync(t => t.ID == id);
            if (tournament == null)
            {
                return NotFound();
            }

            var viewModel = new HomeViewModel(user, tournament, configuration);
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

        [HttpGet("tournaments/test/{count:int}")]
        public IActionResult Test(int count, bool randomize)
        {
            var bracketGenerator = new BracketGenerator();
            var tournament = bracketGenerator.Generate(count);

            if(randomize)
            {
                Randomize();
            }

            var viewModel = new Test(tournament);
            return View(viewModel);

            void Randomize()
            {
                var random = new Random();

                foreach(var round in tournament.Winners)
                {
                    foreach(var game in round.Games)
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

                foreach(var round in tournament.Losers)
                {
                    foreach(var game in round.Games)
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