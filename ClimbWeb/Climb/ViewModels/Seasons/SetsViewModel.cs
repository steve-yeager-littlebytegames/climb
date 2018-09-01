using Climb.Data;
using Climb.Models;

namespace Climb.ViewModels.Seasons
{
    public class SetsViewModel : PageViewModel
    {
        public SetsViewModel(ApplicationUser user, Season season)
            : base(user, season)
        {
        }
    }
}