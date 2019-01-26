using Climb.Models;
using Climb.Requests.Games;
using Climb.Services;
using Microsoft.Extensions.Configuration;

namespace Climb.ViewModels.Games
{
    public class UpdateViewModel : RequestViewModel<UpdateRequest>
    {
        public Game Game { get; }
        public string Action => Game == null ? "Create" : "Update";
        public string LogoImage { get; }
        public string PageTitle => Game == null ? "Create Game" : $"Edit {Game.Name}";

        public UpdateViewModel(ApplicationUser user, Game game, ICdnService cdnService, IConfiguration configuration)
            : base(user, configuration)
        {
            Game = game;
            if(game != null)
            {
                LogoImage = cdnService.GetImageUrl(game.LogoImageKey, ClimbImageRules.GameLogo);
            }
        }
    }
}