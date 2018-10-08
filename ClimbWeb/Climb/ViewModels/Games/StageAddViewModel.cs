using Climb.Data;
using Climb.Models;

namespace Climb.ViewModels.Games
{
    public class StageAddViewModel : BaseViewModel
    {
        public Game Game { get; }
        public Stage Stage { get; }

        public string ActionName => Stage == null ? "Add" : "Update";

        public StageAddViewModel(ApplicationUser user, Game game, Stage stage)
            : base(user)
        {
            Game = game;
            Stage = stage;
        }
    }
}