using System.Collections.Generic;
using System.Linq;
using Climb.Data;
using Climb.Models;
using Microsoft.AspNetCore.Hosting;

namespace Climb.ViewModels.Seasons
{
    public class HomeViewModel : BaseViewModel
    {
        public Season Season { get; }
        public int SeasonNumber { get; }
        public SeasonLeagueUser Participant { get; }
        public bool CanStartSeason { get; }
        public IEnumerable<SeasonLeagueUser> Participants { get; }
        public IEnumerable<Set> AvailableSets { get; }

        public bool CanLeave => Participant != null && !Season.IsComplete;

        private HomeViewModel(ApplicationUser user, Season season, SeasonLeagueUser participant, bool canStartSeason)
            : base(user)
        {
            Season = season;
            Participant = participant;
            SeasonNumber = season.Index + 1;

            Participants = Season.Participants.OrderBy(p => p.Standing);
            AvailableSets = Season.Sets.Where(s => !s.IsComplete).OrderBy(s => s.DueDate);

            CanStartSeason = canStartSeason;
        }

        public static HomeViewModel Create(ApplicationUser user, Season season, IHostingEnvironment environment)
        {
            var participant = user.LeagueUsers
                .FirstOrDefault(lu => season.LeagueID == lu.ID)
                ?.Seasons.FirstOrDefault(slu => slu.SeasonID == season.ID);

            var canStartSeason = false;
            if(!season.IsActive && !season.IsComplete)
            {
                if(environment.IsDevelopment())
                {
                    canStartSeason = true;
                }
                else
                {
                    canStartSeason = season.League.AdminID == user?.Id;
                }
            }

            return new HomeViewModel(user, season, participant, canStartSeason);
        }
    }
}