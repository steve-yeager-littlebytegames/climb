using Climb.Data;
using Climb.Models;

namespace Climb.ViewModels.Tournaments
{
    public class PageViewModel : BaseViewModel
    {
        protected Tournament Tournament { get; }

        public string Name => Tournament.Name;

        public PageViewModel(ApplicationUser user, Tournament tournament)
            : base(user)
        {
            Tournament = tournament;
        }
    }
}