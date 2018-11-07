using Climb.Models;

namespace Climb.Responses.Models
{
    public class TournamentDto
    {
        public int ID { get; }
        public string Name { get; }

        public TournamentDto(Tournament tournament)
        {
            ID = tournament.ID;
            Name = tournament.Name;
        }
    }
}