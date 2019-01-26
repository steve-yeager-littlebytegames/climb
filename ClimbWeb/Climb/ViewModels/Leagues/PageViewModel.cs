using System.Linq;
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

            IsAdmin = IsAdminMode || user?.Id == league.AdminID;
        }

        public bool IsSubPageActive<T>() where T : PageViewModel
        {
            return this is T;
        }
    }
}