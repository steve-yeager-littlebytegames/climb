using System;
using Climb.Models;

namespace Climb.Responses.Models
{
    public class SeasonDto
    {
        public int ID { get; }
        public int LeagueID { get; }
        public int Index { get; }
        public DateTime StartDate { get; }
        public DateTime EndDate { get; }
        public bool IsActive { get; }
        public bool IsComplete { get; }

        public SeasonDto(Season season)
        {
            ID = season.ID;
            LeagueID = season.LeagueID;
            Index = season.Index;
            StartDate = season.StartDate;
            EndDate = season.EndDate;
            IsActive = season.IsActive;
            IsComplete = season.IsComplete;
        }
    }
}