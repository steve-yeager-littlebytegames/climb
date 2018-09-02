using Climb.Data;
using Climb.Models;
using Climb.Requests.Games;
using Climb.Services;

namespace Climb.ViewModels.Games
{
    public class CharacterAddViewModel : RequestViewModel<AddCharacterRequest>
    {
        public Game Game { get; }
        public Character Character { get; }
        public string ImageUrl { get; }

        public string ActionName => Character == null ? "Add" : "Update";

        private CharacterAddViewModel(ApplicationUser user, Game game, Character character, string imageUrl)
            : base(user)
        {
            Game = game;
            Character = character;
            ImageUrl = imageUrl;
        }

        public static CharacterAddViewModel Create(ApplicationUser user, Game game, Character character, ICdnService cdnService)
        {
            var imageKey = character != null
                ? cdnService.GetImageUrl(character.ImageKey, ClimbImageRules.CharacterPic)
                : "/images/NewCharacterIcon.png";

            return new CharacterAddViewModel(user, game, character, imageKey);
        }
    }
}