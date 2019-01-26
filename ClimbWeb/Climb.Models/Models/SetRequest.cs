using System;

namespace Climb.Models
{
    public class SetRequest
    {
        public int ID { get; set; }
        public int LeagueID { get; set; }
        public int RequesterID { get; set; }
        public int ChallengedID { get; set; }
        public DateTime DateCreated { get; set; }
        public int? SetID { get; set; }
        public bool IsOpen { get; set; } = true;
        public string Message { get; set; }

        public League League { get; set; }
        public LeagueUser Requester { get; set; }
        public LeagueUser Challenged { get; set; }
        public Set Set { get; set; }
    }
}
