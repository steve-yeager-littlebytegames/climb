using Climb.Data;
using Climb.Models;
using Microsoft.Extensions.Configuration;

namespace Climb.ViewModels.Leagues
{
    public class ManageViewModel : PageViewModel
    {
        public bool CanStartSeason { get; }

        public ManageViewModel(ApplicationUser user, League league, IConfiguration configuration)
            : base(user, league, configuration)
        {
#if DEBUG
            CanStartSeason = true;
#else
            CanStartSeason = league.AdminID == user?.Id;
#endif
        }
    }
}