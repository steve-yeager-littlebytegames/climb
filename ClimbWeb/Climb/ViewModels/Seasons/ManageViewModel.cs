using System.Linq;
using Climb.Models;
using Climb.Services;
using Microsoft.AspNetCore.Hosting;

namespace Climb.ViewModels.Seasons
{
    public class ManageViewModel : PageViewModel
    {
        public bool CanStartSeason { get; }
        public int OverdueSetCount { get; }

        public ManageViewModel(ApplicationUser user, Season season, IHostingEnvironment environment, IDateService dateService)
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

            OverdueSetCount = season.Sets.Count(s => s.IsOpen && s.DueDate < dateService.Now);
        }
    }
}