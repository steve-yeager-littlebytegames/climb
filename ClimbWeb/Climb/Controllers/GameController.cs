using System;
using System.Threading.Tasks;
using Climb.Data;
using Climb.Models;
using Climb.Requests.Games;
using Climb.Services;
using Climb.Services.ModelServices;
using Climb.ViewModels.Games;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Climb.Controllers
{
    public class GameController : BaseController<GameController>
    {
        private readonly IGameService gameService;
        private readonly ICdnService cdnService;

        public GameController(IGameService gameService, ApplicationDbContext dbContext, ILogger<GameController> logger, IUserManager userManager, ICdnService cdnService)
            : base(logger, userManager, dbContext)
        {
            this.gameService = gameService;
            this.cdnService = cdnService;
        }

        [HttpGet("games")]
        public async Task<IActionResult> Index()
        {
            var user = await GetViewUserAsync();
            var games = await dbContext.Games.Include(g => g.Leagues).ThenInclude(l => l.Members).AsNoTracking().ToArrayAsync();

            var viewModel = new IndexViewModel(user, games);
            return View(viewModel);
        }

        [HttpGet("games/home/{gameID:int}")]
        public async Task<IActionResult> Home(int gameID)
        {
            var user = await GetViewUserAsync();
            var game = await dbContext.Games.Include(g => g.Characters).AsNoTracking().Include(g => g.Stages).AsNoTracking().FirstOrDefaultAsync(g => g.ID == gameID);
            if(game == null)
            {
                return NotFound();
            }

            var viewModel = HomeViewModel.Create(user, game, cdnService);
            return View(viewModel);
        }

        [HttpGet("games/characters/add/{gameID:int}")]
        public async Task<IActionResult> CharacterAdd(int gameID, int? characterID)
        {
            var user = await GetViewUserAsync();

            var game = await dbContext.Games.FirstOrDefaultAsync(g => g.ID == gameID);
            if(game == null)
            {
                return NotFound();
            }

            Character character = null;
            if(characterID != null)
            {
                character = await dbContext.Characters.FirstOrDefaultAsync(c => c.ID == characterID);
                if(character == null)
                {
                    return NotFound();
                }
            }

            var viewModel = CharacterAddViewModel.Create(user, game, character, cdnService);
            return View(viewModel);
        }

        [HttpPost("games/characters/add")]
        public async Task<IActionResult> CharacterAddPost(AddCharacterRequest request)
        {
            try
            {
                await gameService.AddCharacter(request.GameID, request.CharacterID, request.Name, request.Image);
                return RedirectToAction("Home", new {request.GameID});
            }
            catch(Exception exception)
            {
                return GetExceptionResult(exception, request);
            }
        }

        [HttpGet("games/stages/add/{gameID:int}")]
        public async Task<IActionResult> StageAdd(int gameID, int? stageID)
        {
            var user = await GetViewUserAsync();

            var game = await dbContext.Games.FirstOrDefaultAsync(g => g.ID == gameID);
            if(game == null)
            {
                return NotFound();
            }

            Stage stage = null;
            if(stageID != null)
            {
                stage = await dbContext.Stages.FirstOrDefaultAsync(s => s.ID == stageID);
                if(stage == null)
                {
                    return NotFound();
                }
            }

            var viewModel = new StageAddViewModel(user, game, stage);
            return View(viewModel);
        }

        [HttpPost("games/stages/add")]
        public async Task<IActionResult> StageAddPost(AddStageRequest request)
        {
            try
            {
                await gameService.AddStage(request.GameID, request.StageID, request.Name);
                return RedirectToAction("Home", new {request.GameID});
            }
            catch(Exception exception)
            {
                return GetExceptionResult(exception, request);
            }
        }

        [Authorize]
        [HttpGet("games/create")]
        public async Task<IActionResult> Create()
        {
            var user = await GetViewUserAsync();

            var viewModel = new UpdateViewModel(user, null, cdnService);
            return View("Update", viewModel);
        }

        [Authorize]
        [HttpGet("games/update/{gameID:int}")]
        public async Task<IActionResult> Update(int gameID)
        {
            var user = await GetViewUserAsync();

            var game = await dbContext.Games.FirstOrDefaultAsync(g => g.ID == gameID);
            if(game == null)
            {
                return NotFound();
            }

            var viewModel = new UpdateViewModel(user, game, cdnService);
            return View(viewModel);
        }

        [HttpPost("games/update")]
        public async Task<IActionResult> UpdatePost(UpdateRequest request)
        {
            try
            {
                var game = await gameService.Update(request);
                logger.LogInformation($"Game {game.ID} created");

                return RedirectToAction("Home", new
                {
                    gameID = game.ID
                });
            }
            catch(Exception exception)
            {
                return GetExceptionResult(exception, request);
            }
        }
    }
}