using Climb.Data;
using Climb.Models;

namespace Climb.ViewModels.Sets 
{
    public class FightViewModel : BaseViewModel
    {
        public Set Set { get; }

        public FightViewModel(ApplicationUser user, Set set) : base(user)
        {
            Set = set;
        }
    }
}