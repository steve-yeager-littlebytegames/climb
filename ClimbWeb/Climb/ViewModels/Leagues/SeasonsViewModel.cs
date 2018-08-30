using System.Collections.Generic;
using System.Linq;
using Climb.Data;
using Climb.Models;

namespace Climb.ViewModels.Leagues
{
    public class SeasonsViewModel : BaseViewModel
    {
        public League League { get; }
        public bool IsAdmin { get; }
        public Season ActiveSeason { get; }
        public Season NextSeason { get; }
        public IReadOnlyList<Season> CompletedSeasons { get; }

        public SeasonsViewModel(ApplicationUser user, League league)
            : base(user)
        {
            League = league;

            IsAdmin = league.AdminID == user?.Id;
            ActiveSeason = league.ActiveSeason;
            NextSeason = league.Seasons.FirstOrDefault(s => !s.IsComplete && !s.IsActive);
            CompletedSeasons = league.Seasons.Where(s => s.IsComplete).ToArray();
        }
    }
}