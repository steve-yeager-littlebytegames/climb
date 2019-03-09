using Climb.Models;
using Climb.Requests.Games;
using Climb.Services;

namespace Climb.ViewModels.Games
{
    public class UpdateViewModel : RequestViewModel<UpdateRequest>
    {
        public Game Game { get; }
        public string Action => Game == null ? "Create" : "Update";
        public string LogoImage { get; }
        public string BannerImage { get; }
        public string PageTitle => Game == null ? "Create Game" : $"Edit {Game.Name}";

        public UpdateViewModel(ApplicationUser user, Game game, ICdnService cdnService)
            : base(user)
        {
            Game = game;
            if(game != null)
            {
                LogoImage = cdnService.GetImageUrl(game.LogoImageKey, ClimbImageRules.GameLogo);
            }
        }
    }
}