using Climb.Data;
using Climb.Models;
using Microsoft.AspNetCore.Hosting;

namespace Climb.ViewModels.Seasons
{
    public class MembershipViewModel : PageViewModel
    {
        public bool CanJoin => Participant == null && User != null && Member != null;
        public bool CanLeave => Participant != null && !Participant.HasLeft && !Season.IsComplete;

        public MembershipViewModel(ApplicationUser user, Season season, IHostingEnvironment environment)
            : base(user, season, environment)
        {
        }
    }
}