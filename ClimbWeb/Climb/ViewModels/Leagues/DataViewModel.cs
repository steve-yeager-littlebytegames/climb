using Climb.Data;
using Climb.Models;

namespace Climb.ViewModels.Leagues
{
    public class DataViewModel : PageViewModel
    {
        public DataViewModel(ApplicationUser user, League league)
            : base(user, league)
        {
        }
    }
}