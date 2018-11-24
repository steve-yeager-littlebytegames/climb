using System.Collections.Generic;
using Climb.Data;
using Climb.Models;
using Microsoft.Extensions.Configuration;

namespace Climb.ViewModels.Games
{
    public class IndexViewModel : BaseViewModel
    {
        public IReadOnlyList<Game> AllGames { get; }

        public IndexViewModel(ApplicationUser user, IReadOnlyList<Game> allGames, IConfiguration configuration)
            : base(user, configuration)
        {
            AllGames = allGames;
        }
    }
}