using System.Collections.Generic;
using Climb.Data;
using Climb.Models;
using Microsoft.Extensions.Configuration;

namespace Climb.ViewModels.Leagues
{
    public class IndexViewModel : BaseViewModel
    {
        public IReadOnlyList<League> AllLeagues { get; }
        public IReadOnlyList<Game> AllGames { get; }

        public IndexViewModel(ApplicationUser user, IReadOnlyList<League> allLeagues, IReadOnlyList<Game> allGames, IConfiguration configuration)
            : base(user, configuration)
        {
            AllLeagues = allLeagues;
            AllGames = allGames;
        }
    }
}