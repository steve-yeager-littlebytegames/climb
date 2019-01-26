using Climb.Models;
using Microsoft.Extensions.Configuration;

namespace Climb.ViewModels.Games
{
    public class StageAddViewModel : BaseViewModel
    {
        public Game Game { get; }
        public Stage Stage { get; }

        public string ActionName => Stage == null ? "Add" : "Update";

        public StageAddViewModel(ApplicationUser user, Game game, Stage stage, IConfiguration configuration)
            : base(user, configuration)
        {
            Game = game;
            Stage = stage;
        }
    }
}