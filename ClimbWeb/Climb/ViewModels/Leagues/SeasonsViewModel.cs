using System.Collections.Generic;
using System.Linq;
using Climb.Data;
using Climb.Models;
using Microsoft.Extensions.Configuration;

namespace Climb.ViewModels.Leagues
{
    public class SeasonsViewModel : PageViewModel
    {
        public Season ActiveSeason { get; }
        public Season NextSeason { get; }
        public IReadOnlyList<Season> CompletedSeasons { get; }

        public SeasonsViewModel(ApplicationUser user, League league, IConfiguration configuration)
            : base(user, league, configuration)
        {
            ActiveSeason = league.ActiveSeason;
            NextSeason = league.Seasons.FirstOrDefault(s => !s.IsComplete && !s.IsActive);
            CompletedSeasons = league.Seasons.Where(s => s.IsComplete).ToArray();
        }
    }
}