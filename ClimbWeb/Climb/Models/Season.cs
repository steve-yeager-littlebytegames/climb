using System;
using System.Collections.Generic;

namespace Climb.Models
{
    public class Season
    {
        public int ID { get; set; }
        public int LeagueID { get; set; }
        public int Index { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsComplete { get; set; }

        public League League { get; set; }
        public List<SeasonLeagueUser> Participants { get; set; }
        public List<Set> Sets { get; set; }

        public Season()
        {
        }

        public Season(int leagueID, int index, DateTime start, DateTime end)
        {
            LeagueID = leagueID;
            Index = index;
            StartDate = start;
            EndDate = end;
        }
    }
}