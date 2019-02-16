using Climb.Models;

namespace Climb.Responses.Models
{
    public class TournamentUserDto
    {
        public int ID { get; }
        public string UserID { get; }
        public string DisplayName { get; }
        public int Seed { get; }

        public TournamentUserDto(TournamentUser user)
        {
            ID = user.ID;
            UserID = user.UserID;
            Seed = user.Seed;
            DisplayName = user?.LeagueUser?.DisplayName;
        }
    }
}