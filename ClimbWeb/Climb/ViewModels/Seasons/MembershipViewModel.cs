using Climb.Data;
using Climb.Models;
using Microsoft.Extensions.Configuration;

namespace Climb.ViewModels.Seasons
{
    public class MembershipViewModel : PageViewModel
    {
        public bool CanLeave => Participant != null && !Participant.HasLeft && !Season.IsComplete;

        public MembershipViewModel(ApplicationUser user, Season season, IConfiguration configuration)
            : base(user, season, configuration)
        {
        }
    }
}