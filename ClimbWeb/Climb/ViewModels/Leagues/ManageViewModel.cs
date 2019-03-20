using Climb.Models;
using Climb.Requests.Seasons;
using JetBrains.Annotations;

namespace Climb.ViewModels.Leagues
{
    public class ManageViewModel : PageViewModel, IRequestViewModel<CreateRequest>
    {
        [UsedImplicitly]
        public CreateRequest Request { get; }
        public bool CanStartSeason { get; }

        public ManageViewModel(ApplicationUser user, League league)
            : base(user, league)
        {
            CanStartSeason = league.AdminID == user?.Id;
        }

    }
}