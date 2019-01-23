using Climb.Models;
using Microsoft.AspNetCore.Hosting;

namespace Climb.ViewModels.Seasons
{
    public class ManageViewModel : PageViewModel
    {
        public bool CanStartSeason { get; }

        public ManageViewModel(ApplicationUser user, Season season, IHostingEnvironment environment)
            : base(user, season, environment)
        {
            if(!season.IsActive && !season.IsComplete)
            {
                if(environment.IsDevelopment())
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