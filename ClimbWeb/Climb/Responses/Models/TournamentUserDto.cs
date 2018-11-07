using Climb.Models;

namespace Climb.Responses.Models
{
    public class TournamentUserDto
    {
        public int ID { get; set; }
        public string UserID { get; set; }
        public int Seed { get; set; }

        public TournamentUserDto(TournamentUser user)
        {
            ID = user.ID;
            UserID = user.UserID;
            Seed = user.Seed;
        }
    }
}