using Climb.Models;
using Microsoft.Extensions.Configuration;

namespace Climb.ViewModels.Seasons
{
    public class ManageViewModel : PageViewModel
    {
        public bool CanStartSeason { get; }

        public ManageViewModel(ApplicationUser user, Season season, IConfiguration configuration)
            : base(user, season, configuration)
        {
            if(!season.IsActive && !season.IsComplete)
            {
                CanStartSeason = IsAdminMode || season.League.AdminID == user?.Id;
            }
        }
    }
}