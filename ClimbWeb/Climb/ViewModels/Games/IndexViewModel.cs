using System.Collections.Generic;
using Climb.Models;

namespace Climb.ViewModels.Games
{
    public class IndexViewModel : BaseViewModel
    {
        public IReadOnlyList<Game> AllGames { get; }

        public IndexViewModel(ApplicationUser user, IReadOnlyList<Game> allGames)
            : base(user)
        {
            AllGames = allGames;
        }
    }
}