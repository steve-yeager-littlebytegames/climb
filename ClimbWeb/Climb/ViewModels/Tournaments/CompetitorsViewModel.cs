using System.Collections.Generic;
using Climb.Models;
using Microsoft.Extensions.Configuration;

namespace Climb.ViewModels.Tournaments
{
    public class CompetitorsViewModel : PageViewModel
    {
        public IReadOnlyList<TournamentUser> Competitors { get; }

        public CompetitorsViewModel(ApplicationUser user, Tournament tournament, IConfiguration configuration)
            : base(user, tournament, configuration)
        {
            tournament.TournamentUsers.Sort((x, y) => x.Seed.CompareTo(y.Seed));
            Competitors = tournament.TournamentUsers;
        }
    }
}