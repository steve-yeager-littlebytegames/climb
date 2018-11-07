using Climb.Data;

namespace Climb.Models
{
    public class TournamentUser
    {
        public int ID { get; set; }
        public string UserID { get; set; }
        public int LeagueUserID { get; set; }
        public int? SeasonLeagueUserID { get; set; }
        public int TournamentID { get; set; }
        public int Seed { get; set; }

        public ApplicationUser User { get; set; }
        public LeagueUser LeagueUser { get; set; }
        public SeasonLeagueUser SeasonLeagueUser { get; set; }
        public Tournament Tournament { get; set; }

        public TournamentUser()
        {
        }

        public TournamentUser(SeasonLeagueUser seasonUser)
        {
            UserID = seasonUser.UserID;
            LeagueUserID = seasonUser.LeagueUserID;
            SeasonLeagueUserID = seasonUser.ID;
        }
    }
}