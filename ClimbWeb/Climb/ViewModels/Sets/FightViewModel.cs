using Climb.Data;
using Climb.Models;

namespace Climb.ViewModels.Sets 
{
    public class FightViewModel : BaseViewModel
    {
        public Set Set { get; }
        public string Referer { get; }

        public FightViewModel(ApplicationUser user, Set set, string referer) : base(user)
        {
            Set = set;
            Referer = referer;
        }
    }
}