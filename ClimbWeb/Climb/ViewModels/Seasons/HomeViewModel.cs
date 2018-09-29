﻿using System.Collections.Generic;
using System.Linq;
using Climb.Data;
using Climb.Models;
using Microsoft.AspNetCore.Hosting;

namespace Climb.ViewModels.Seasons
{
    public class HomeViewModel : PageViewModel
    {
        public IEnumerable<SeasonLeagueUser> Participants { get; }
        public IReadOnlyList<Set> RecentSets { get; }
        public IEnumerable<Set> AvailableSets { get; }

        public HomeViewModel(ApplicationUser user, Season season, IHostingEnvironment environment)
            : base(user, season, environment)
        {
            Participants = Season.Participants.OrderBy(p => p.Standing);
            RecentSets = Season.Sets.Where(s => s.IsComplete).OrderByDescending(s => s.UpdatedDate).Take(10).ToArray();
            AvailableSets = Season.Sets.Where(s => !s.IsComplete).OrderBy(s => s.DueDate);
        }
    }
}