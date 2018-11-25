using Climb.Data;
using Climb.Models;
using Microsoft.Extensions.Configuration;

namespace Climb.ViewModels.Tournaments
{
    public class ManageViewModel : PageViewModel
    {
        public ManageViewModel(ApplicationUser user, Tournament tournament, IConfiguration configuration)
            : base(user, tournament, configuration)
        {
        }
    }
}