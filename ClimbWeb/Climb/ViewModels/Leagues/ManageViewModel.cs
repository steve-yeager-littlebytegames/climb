using Climb.Models;

namespace Climb.ViewModels.Leagues
{
    public class ManageViewModel : PageViewModel
    {
        public bool CanStartSeason { get; }

        public ManageViewModel(ApplicationUser user, League league)
            : base(user, league)
        {
#if DEBUG
            CanStartSeason = true;
#else
            CanStartSeason = league.AdminID == user?.Id;
#endif
        }
    }
}