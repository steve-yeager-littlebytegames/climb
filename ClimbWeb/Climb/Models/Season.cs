using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Climb.Models
{
    public class Season
    {
        [UsedImplicitly]
        public int ID { get; private set; }
        [UsedImplicitly]
        public int LeagueID { get; private set; }
        [UsedImplicitly]
        public int Index { get; private set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsComplete { get; set; }

        [UsedImplicitly]
        public League League { get; private set; }
        [UsedImplicitly]
        public List<SeasonLeagueUser> Participants { get; private set; }
        [UsedImplicitly]
        public List<Set> Sets { get; private set; }

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