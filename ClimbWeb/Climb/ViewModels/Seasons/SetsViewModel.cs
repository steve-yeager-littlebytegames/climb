using Climb.Models;
using Microsoft.Extensions.Configuration;

namespace Climb.ViewModels.Seasons
{
    public class SetsViewModel : PageViewModel
    {
        public SetsViewModel(ApplicationUser user, Season season, IConfiguration configuration)
            : base(user, season, configuration)
        {
        }
    }
}