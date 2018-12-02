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

        public TournamentUser(Tournament tournament, string userID, int leagueUserID, int? seasonUserID = null)
        {
            Tournament = tournament;
            UserID = userID;
            LeagueUserID = leagueUserID;
            SeasonLeagueUserID = seasonUserID;
        }
    }
}