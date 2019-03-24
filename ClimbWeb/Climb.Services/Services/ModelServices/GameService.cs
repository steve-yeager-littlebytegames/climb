using System.Collections.Generic;
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
        private readonly IDateService dateService;

        public GameService(ApplicationDbContext dbContext, ICdnService cdnService, IDateService dateService)
        {
            this.dbContext = dbContext;
            this.cdnService = cdnService;
            this.dateService = dateService;
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

                if(request.BannerImage == null)
                {
                    throw new BadRequestException(nameof(request.BannerImage), "A game needs a banner.");
                }

                var logoImageKey = await cdnService.UploadImageAsync(request.LogoImage, ClimbImageRules.GameLogo);
                var bannerImageKey = await cdnService.UploadImageAsync(request.BannerImage, ClimbImageRules.GameBanner);

                game = new Game(request.Name, request.CharactersPerMatch, request.MaxMatchPoints, dateService.Now)
                {
                    LogoImageKey = logoImageKey,
                    BannerImageKey = bannerImageKey,
                };
                dbContext.Add(game);
            }
            else
            {
                game.Name = request.Name;
                game.CharactersPerMatch = request.CharactersPerMatch;
                game.MaxMatchPoints = request.MaxMatchPoints;

                game.LogoImageKey = await TryUpdateImage(request.LogoImage, game.LogoImageKey, ClimbImageRules.GameLogo);
                game.BannerImageKey = await TryUpdateImage(request.BannerImage, game.BannerImageKey, ClimbImageRules.GameBanner);

                dbContext.Update(game);
            }

            game.ScoreName = string.IsNullOrWhiteSpace(request.ScoreName) ? Game.DefaultScoreName : request.ScoreName;
            game.MatchName = string.IsNullOrWhiteSpace(request.MatchName) ? Game.DefaultMatchName : request.MatchName;

            await dbContext.SaveChangesAsync();

            return game;

            async Task<string> TryUpdateImage(IFormFile image, string oldKey, ImageRules imageRules)
            {
                if(image != null)
                {
                    var newKey = await cdnService.ReplaceImageAsync(oldKey, image, imageRules);
                    return newKey;
                }

                return oldKey;
            }
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
                if(game.Characters.Any(c => c.Name == name))
                {
                    throw new ConflictException(typeof(Character), nameof(Character.Name), name);
                }

                string imageKey = null;
                if(imageFile != null)
                {
                    imageKey = await cdnService.UploadImageAsync(imageFile, ClimbImageRules.CharacterPic);
                }

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

        public async Task<IReadOnlyCollection<Character>> AddCharacters(int gameID, ICollection<string> names)
        {
            var characters = await dbContext.Characters.Where(c => c.GameID == gameID).ToArrayAsync();
            var addedCharacters = new List<Character>(names.Count);

            foreach(var name in names)
            {
                if(characters.Any(c => c.Name == name))
                {
                    continue;
                }

                var character = await AddCharacter(gameID, null, name, null);
                addedCharacters.Add(character);
            }

            return addedCharacters;
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