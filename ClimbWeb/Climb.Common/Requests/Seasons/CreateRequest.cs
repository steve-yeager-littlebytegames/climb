using System;
using System.ComponentModel.DataAnnotations;

namespace Climb.Requests.Seasons
{
    public class CreateRequest
    {
        [Required]
        public int LeagueID { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }

        public CreateRequest()
        {
        }

        public CreateRequest(int leagueID, DateTime start, DateTime end)
        {
            LeagueID = leagueID;
            StartDate = start;
            EndDate = end;
        }
    }
}