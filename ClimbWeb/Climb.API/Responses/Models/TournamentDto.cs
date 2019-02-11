using System.Linq;
using Climb.Models;

namespace Climb.Responses.Models
{
    public class TournamentDto
    {
        public int ID { get; }
        public string Name { get; }
        public TournamentUserDto[] Competitors { get; }

        public TournamentDto(Tournament tournament)
        {
            ID = tournament.ID;
            Name = tournament.Name;

            if(tournament.TournamentUsers != null)
            {
                Competitors = tournament.TournamentUsers
                    .Select(tu => new TournamentUserDto(tu))
                    .OrderBy(tu => tu.Seed)
                    .ToArray();
            }
        }
    }
}