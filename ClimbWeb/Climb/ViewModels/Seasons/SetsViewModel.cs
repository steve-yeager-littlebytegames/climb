using Climb.Data;
using Climb.Models;
using Microsoft.AspNetCore.Hosting;

namespace Climb.ViewModels.Seasons
{
    public class SetsViewModel : PageViewModel
    {
        public SetsViewModel(ApplicationUser user, Season season, IHostingEnvironment environment)
            : base(user, season, environment)
        {
        }
    }
}