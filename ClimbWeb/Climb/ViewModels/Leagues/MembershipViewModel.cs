﻿using System.Collections.Generic;
using System.Linq;
using Climb.Models;

namespace Climb.ViewModels.Leagues
{
    public class MembershipViewModel : PageViewModel
    {
        public IReadOnlyList<LeagueUser> Members { get; }
        public IReadOnlyList<LeagueUser> Newcomers { get; }
        public bool CanStartSeason { get; }

        public MembershipViewModel(ApplicationUser user, League league)
            : base(user, league)
        {
            league.Members.Sort();
            Members = league.Members.Where(lu => !lu.IsNewcomer).ToList();
            Newcomers = league.Members.Where(lu => lu.IsNewcomer).ToList();

#if DEBUG
            CanStartSeason = true;
#else
            CanStartSeason = league.AdminID == user?.Id;
#endif
        }
    }
}