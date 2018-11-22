using Climb.Data;
using Climb.Models;

namespace Climb.ViewModels.Tournaments
{
    public class ManageViewModel : PageViewModel
    {
        public ManageViewModel(ApplicationUser user, Tournament tournament)
            : base(user, tournament)
        {
        }
    }
}