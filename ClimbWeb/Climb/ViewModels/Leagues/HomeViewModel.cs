using System.Collections.Generic;
using System.Linq;
using Climb.Data;
using Climb.Models;

namespace Climb.ViewModels.Leagues
{
    public class HomeViewModel : BaseViewModel
    {
        public League League { get; }
        public bool IsMember { get; }
        public IReadOnlyList<LeagueUser> Members { get; }
        public IReadOnlyList<LeagueUser> Newcomers { get; }
        public bool CanStartSeason { get;  }

        public HomeViewModel(ApplicationUser user, League league)
            : base(user)
        {
            League = league;

            league.Members.Sort();
            Members = league.Members.Where(lu => !lu.IsNewcomer).ToList();
            Newcomers = league.Members.Where(lu => lu.IsNewcomer).ToList();
            IsMember = league.Members.Any(lu => lu.UserID == user?.Id);

#if DEBUG
            CanStartSeason = true;
#else
            CanStartSeason = league.AdminID == user.Id;
#endif
        }
    }
}