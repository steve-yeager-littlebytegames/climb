using System;
using Climb.Data;
using Climb.Models;
using Climb.Services;

namespace Climb.ViewModels.Games
{
    public class HomeViewModel : BaseViewModel
    {
        public Game Game { get; }
        public string LogoUrl { get; }

        private HomeViewModel(ApplicationUser user, Game game, string logoUrl)
            : base(user)
        {
            Game = game;
            LogoUrl = logoUrl;
        }

        public static HomeViewModel Create(ApplicationUser user, Game game, ICdnService cdnService)
        {
            game.Characters.Sort((a, b) => string.Compare(a.Name, b.Name, StringComparison.OrdinalIgnoreCase));
            game.Stages.Sort((a, b) => string.Compare(a.Name, b.Name, StringComparison.OrdinalIgnoreCase));

            var logoUrl = cdnService.GetImageUrl(game.LogoImageKey, ClimbImageRules.GameLogo);

            return new HomeViewModel(user, game, logoUrl);
        }
    }
}