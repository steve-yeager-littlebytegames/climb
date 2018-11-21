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
            tournament.TournamentUsers.Sort((x, y) => x.Seed.CompareTo(y.Seed));
            Competitors = tournament.TournamentUsers;
        }
    }
}