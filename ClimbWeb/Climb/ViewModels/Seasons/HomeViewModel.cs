using System.Collections.Generic;
using System.Linq;
using Climb.Data;
using Climb.Models;

namespace Climb.ViewModels.Seasons
{
    public class HomeViewModel : BaseViewModel
    {
        public Season Season { get; }
        public int SeasonNumber { get; }
        public bool IsParticipant { get; }
        public bool CanStartSeason { get; }
        public IEnumerable<SeasonLeagueUser> Participants { get; }
        public IEnumerable<Set> AvailableSets { get; }

        private HomeViewModel(ApplicationUser user, Season season, bool isParticipant)
            : base(user)
        {
            Season = season;
            IsParticipant = isParticipant;
            SeasonNumber = season.Index + 1;

            Participants = Season.Participants.OrderBy(p => p.Standing);
            AvailableSets = Season.Sets.Where(s => !s.IsComplete).OrderBy(s => s.DueDate);

            if(season.IsActive)
            {
                CanStartSeason = false;
            }
            else
            {
#if DEBUG
                CanStartSeason = true;
#else
                CanStartSeason = season.League.AdminID == user?.Id;
#endif
            }
        }

        public static HomeViewModel Create(ApplicationUser user, Season season)
        {
            var isParticipant = user.LeagueUsers.Any(lu => season.Participants.Any(slu => slu.LeagueUserID == lu.ID));

            return new HomeViewModel(user, season, isParticipant);
        }
    }
}