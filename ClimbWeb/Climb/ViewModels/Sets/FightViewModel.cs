using Climb.Models;
using Microsoft.Extensions.Configuration;

namespace Climb.ViewModels.Sets
{
    public class FightViewModel : BaseViewModel
    {
        public Set Set { get; }
        public string Referer { get; }

        public FightViewModel(ApplicationUser user, Set set, string referer, IConfiguration configuration)
            : base(user, configuration)
        {
            Set = set;
            Referer = referer;
        }
    }
}