using System.Linq;
using Climb.Extensions;
using Climb.Models;
using Microsoft.Extensions.Configuration;

namespace Climb.ViewModels.Seasons
{
    public abstract class PageViewModel : BaseViewModel
    {
        public Season Season { get; }
        public SeasonLeagueUser Participant { get; }
        private LeagueUser Member { get; }
        public bool CanManage { get; }

        public int SeasonNumber => Season.Index + 1;
        public bool IsParticipant => Participant != null;
        public bool CanJoin => Participant == null && User != null && Member != null && !Season.IsComplete;

        protected PageViewModel(ApplicationUser user, Season season, IConfiguration configuration)
            : base(user, configuration)
        {
            Season = season;

            if(user != null)
            {
                Participant = season.Participants.FirstOrDefault(slu => slu.UserID == user.Id);
                Member = season.League.Members.FirstOrDefault(lu => lu.UserID == user.Id);
            }

            CanManage = IsAdminMode || Season.League.AdminID == Participant?.UserID;
        }
    }
}