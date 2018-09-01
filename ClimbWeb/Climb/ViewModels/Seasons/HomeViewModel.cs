using System.Collections.Generic;
using System.Linq;
using Climb.Data;
using Climb.Models;

namespace Climb.ViewModels.Seasons
{
    public class HomeViewModel : PageViewModel
    {
        public IEnumerable<SeasonLeagueUser> Participants { get; }
        public IEnumerable<Set> AvailableSets { get; }

        public HomeViewModel(ApplicationUser user, Season season)
            : base(user, season)
        {
            Participants = Season.Participants.OrderBy(p => p.Standing);
            AvailableSets = Season.Sets.Where(s => !s.IsComplete).OrderBy(s => s.DueDate);
        }
    }
}