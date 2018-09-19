using System.Collections.Generic;
using Climb.Data;
using Climb.Models;

namespace Climb.ViewModels.Leagues
{
    public class IndexViewModel : BaseViewModel
    {
        public IReadOnlyList<League> AllLeagues { get; }
        public IReadOnlyList<Game> AllGames { get; }

        public IndexViewModel(ApplicationUser user, IReadOnlyList<League> allLeagues, IReadOnlyList<Game> allGames)
            : base(user)
        {
            AllLeagues = allLeagues;
            AllGames = allGames;
        }
    }
}