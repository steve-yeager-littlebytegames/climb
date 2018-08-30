using System.Collections.Generic;
using System.Linq;
using Climb.Data;
using Climb.Models;

namespace Climb.ViewModels.Leagues
{
    public class HomeViewModel : BaseViewModel
    {
        public League League { get; }
        public bool IsAdmin { get; }
        public IReadOnlyList<LeagueUser> Members { get; }
        public IReadOnlyList<LeagueUser> Newcomers { get; }
        public bool CanStartSeason { get; }
        public LeagueUser Member { get; }

        public bool IsMember => Member != null;

        public HomeViewModel(ApplicationUser user, League league)
            : base(user)
        {
            League = league;

            league.Members.Sort();
            Members = league.Members.Where(lu => !lu.IsNewcomer).ToList();
            Newcomers = league.Members.Where(lu => lu.IsNewcomer).ToList();
            Member = league.Members.FirstOrDefault(lu => lu.UserID == user?.Id);

#if DEBUG
            IsAdmin = true;
#else
            IsAdmin = user?.Id == league.AdminID;
#endif

#if DEBUG
            CanStartSeason = true;
#else
            CanStartSeason = league.AdminID == user?.Id;
#endif
        }
    }
}