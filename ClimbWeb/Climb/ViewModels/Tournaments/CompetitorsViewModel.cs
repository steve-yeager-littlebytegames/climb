using System.Collections.Generic;
using Climb.Data;
using Climb.Models;

namespace Climb.ViewModels.Tournaments
{
    public class CompetitorsViewModel : PageViewModel
    {
        public IReadOnlyList<TournamentUser> Competitors { get; }

        public CompetitorsViewModel(ApplicationUser user, Tournament tournament)
            : base(user, tournament)
        {
            Competitors = tournament.TournamentUsers;
        }
    }
}