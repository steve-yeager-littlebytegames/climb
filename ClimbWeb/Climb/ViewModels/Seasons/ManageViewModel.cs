using Climb.Data;
using Climb.Extensions;
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
                if(configuration.IsDevMode(DevModes.Admin))
                {
                    CanStartSeason = true;
                }
                else
                {
                    CanStartSeason = season.League.AdminID == user?.Id;
                }
            }
        }
    }
}