using System;
using System.Linq;
using System.Threading.Tasks;
using Climb.Data;
using Climb.Exceptions;
using Climb.Models;
using Climb.Requests.Games;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Climb.Services.ModelServices
{
    public class GameService : IGameService
    {
        private readonly ApplicationDbContext dbContext;
        private readonly ICdnService cdnService;

        public GameService(ApplicationDbContext dbContext, ICdnService cdnService)
        {
            this.dbContext = dbContext;
            this.cdnService = cdnService;
        }

        public async Task<Game> Update(UpdateRequest request)
        {
            if(request.CharactersPerMatch < 1)
            {
                throw new BadRequestException(nameof(request.CharactersPerMatch), "A game needs at least 1 character per match");
            }

            if(request.MaxMatchPoints < 1)
            {
                throw new BadRequestException(nameof(request.MaxMatchPoints), "A game needs at least 1 point per match.");
            }

            if(await dbContext.Games.AnyAsync(g => g.Name == request.Name && (request.GameID == null || g.ID != request.GameID)))
            {
                throw new ConflictException(typeof(Game), nameof(Game.Name), request.Name);
            }

            var game = await dbContext.Games.FirstOrDefaultAsync(g => g.ID == request.GameID);

            if(game == null)
            {
                if(request.LogoImage == null)
                {
                    throw new BadRequestException(nameof(request.LogoImage), "A game needs a logo.");
                }

                var logoImageKey = await cdnService.UploadImageAsync(request.LogoImage, ClimbImageRules.GameLogo);

                game = new Game(request.Name, request.CharactersPerMatch, request.MaxMatchPoints, request.HasStages)
                {
                    LogoImageKey = logoImageKey
                };
                dbContext.Add(game);
            }
            else
            {
                game.Name = request.Name;
                game.CharactersPerMatch = request.CharactersPerMatch;
                game.MaxMatchPoints = request.MaxMatchPoints;
                game.HasStages = request.HasStages;

                if(request.LogoImage != null)
                {
                    var logoKey = await cdnService.ReplaceImageAsync(game.LogoImageKey, request.LogoImage, ClimbImageRules.GameLogo);
                    game.LogoImageKey = logoKey;
                }

                dbContext.Update(game);
            }
            
            await dbContext.SaveChangesAsync();

            return game;
        }

        public async Task<Character> AddCharacter(int gameID, int? characterID, string name, IFormFile imageFile)
        {
            var game = await dbContext.Games
                .Include(g => g.Characters).AsNoTracking()
                .FirstOrDefaultAsync(g => g.ID == gameID);
            if(game == null)
            {
                throw new NotFoundException(typeof(Game), gameID);
            }

            Character character;
            if(characterID == null)
            {
                if(imageFile == null)
                {
                    throw new ArgumentNullException(nameof(imageFile));
                }

                if(game.Characters.Any(c => c.Name == name))
                {
                    throw new ConflictException(typeof(Character), nameof(Character.Name), name);
                }

                var imageKey = await cdnService.UploadImageAsync(imageFile, ClimbImageRules.CharacterPic);

                character = new Character
                {
                    Name = name,
                    GameID = gameID,
                    ImageKey = imageKey,
                };

                dbContext.Add(character);
            }
            else
            {
                character = await dbContext.Characters.FirstOrDefaultAsync(c => c.ID == characterID);
                if(character == null)
                {
                    throw new NotFoundException(typeof(Character), characterID.Value);
                }

                dbContext.Update(character);
                character.Name = name;

                if(imageFile != null)
                {
                    var imageKey = await cdnService.ReplaceImageAsync(game.LogoImageKey, imageFile, ClimbImageRules.CharacterPic);
                    character.ImageKey = imageKey;
                }
            }

            await dbContext.SaveChangesAsync();

            return character;
        }

        public async Task<Stage> AddStage(int gameID, int? stageID, string name)
        {
            var game = await dbContext.Games
                .Include(g => g.Stages).AsNoTracking()
                .FirstOrDefaultAsync(g => g.ID == gameID);
            if(game == null)
            {
                throw new NotFoundException(typeof(Game), gameID);
            }

            Stage stage;
            if(stageID == null)
            {
                if(game.Stages.Any(c => c.Name == name))
                {
                    throw new ConflictException(typeof(Stage), nameof(Stage.Name), name);
                }

                stage = new Stage
                {
                    Name = name,
                    GameID = gameID,
                };

                dbContext.Add(stage);
            }
            else
            {
                stage = await dbContext.Stages.FirstOrDefaultAsync(s => s.ID == stageID);
                if(stage == null)
                {
                    throw new NotFoundException(typeof(Stage), stageID.Value);
                }

                dbContext.Update(stage);

                stage.Name = name;
            }

            await dbContext.SaveChangesAsync();

            return stage;
        }
    }
}