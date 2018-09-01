using System.Linq;
using Climb.Data;
using Climb.Models;

namespace Climb.ViewModels.Seasons
{
    public abstract class PageViewModel : BaseViewModel
    {
        public Season Season { get; }
        public SeasonLeagueUser Participant { get; }
        protected LeagueUser Member { get; }

        public int SeasonNumber => Season.Index + 1;

        protected PageViewModel(ApplicationUser user, Season season)
            : base(user)
        {
            Season = season;

            if (user != null)
            {
                Participant = season.Participants.FirstOrDefault(slu => slu.UserID == user.Id);
                Member = season.League.Members.FirstOrDefault(lu => lu.UserID == user.Id);
            }
        }
    }
}