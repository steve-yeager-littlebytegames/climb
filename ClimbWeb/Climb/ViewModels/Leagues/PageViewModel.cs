using System.Linq;
using Climb.Models;

namespace Climb.ViewModels.Leagues
{
    public abstract class PageViewModel : BaseViewModel
    {
        public League League { get; }
        public LeagueUser Member { get; }
        public bool IsAdmin { get; }

        public bool IsMember => Member != null;

        protected PageViewModel(ApplicationUser user, League league)
            : base(user)
        {
            League = league;
            Member = league.Members.FirstOrDefault(lu => lu.UserID == user?.Id);

#if DEBUG
            IsAdmin = true;
#else
            IsAdmin = user?.Id == league.AdminID;
#endif
        }

        public bool IsSubPageActive<T>() where T : PageViewModel
        {
            return this is T;
        }
    }
}