using Climb.Data;
using Climb.Models;
using Microsoft.Extensions.Configuration;

namespace Climb.ViewModels.Tournaments
{
    public class PageViewModel : BaseViewModel
    {
        public Tournament Tournament { get; }

        public string Name => Tournament.Name;

        protected PageViewModel(ApplicationUser user, Tournament tournament, IConfiguration configuration)
            : base(user, configuration)
        {
            Tournament = tournament;
        }
    }
}