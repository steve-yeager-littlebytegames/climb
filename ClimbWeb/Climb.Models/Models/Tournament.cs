using System;
using System.Collections.Generic;

namespace Climb.Models
{
    public class Tournament
    {
        public enum States
        {
            Open,
            Active,
            Complete,
        }

        public int ID { get; set; }
        public string Name { get; set; }
        public int LeagueID { get; set; }
        public int? SeasonID { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public States State { get; set; }

        public League League { get; set; }
        public Season Season { get; set; }
        public List<LeagueUser> LeagueUsers { get; set; }
        public List<Round> Rounds { get; set; }
        public List<SetSlot> SetSlots { get; set; }
        public List<Set> Sets { get; set; }
        public List<TournamentUser> TournamentUsers { get; set; }

        public Tournament()
        {
        }

        public Tournament(int leagueID, DateTime createDate)
        {
            LeagueID = leagueID;
            CreateDate = createDate;
        }
    }
}