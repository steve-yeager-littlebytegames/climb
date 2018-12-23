using System.ComponentModel.DataAnnotations;

namespace Climb.Requests.Leagues
{
    public class JoinRequest
    {
        [Required]
        public int LeagueID { get; set; }
        [Required]
        public string UserID { get; set; }

        public JoinRequest()
        {
        }

        public JoinRequest(int leagueID, string userID)
        {
            LeagueID = leagueID;
            UserID = userID;
        }
    }
}