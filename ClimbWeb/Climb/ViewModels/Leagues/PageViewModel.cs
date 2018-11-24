﻿using System.Linq;
using Climb.Data;
using Climb.Models;
using Microsoft.Extensions.Configuration;

namespace Climb.ViewModels.Leagues
{
    public abstract class PageViewModel : BaseViewModel
    {
        public League League { get; }
        public LeagueUser Member { get; }
        public bool IsAdmin { get; }

        public bool IsMember => Member != null;

        protected PageViewModel(ApplicationUser user, League league, IConfiguration configuration)
            : base(user, configuration)
        {
            League = league;
            Member = league.Members.FirstOrDefault(lu => lu.UserID == user?.Id);

#if DEBUG
            IsAdmin = true;
#else
            IsAdmin = user?.Id == league.AdminID;
#endif
        }
    }
}