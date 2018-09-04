using System;
using Climb.Models;

namespace Climb.Responses.Models
{
    public class LeagueDto
    {
        public int ID { get; }
        public int GameID { get; }
        public int? OrganizationID { get; }
        public string Name { get; }
        public int SetsTillRank { get; }
        public DateTime DateCreated { get; }
        public string AdminID { get; }
        public int? ActiveSeasonID { get; }

        public LeagueDto(League league)
        {
            ID = league.ID;
            GameID = league.GameID;
            OrganizationID = league.OrganizationID;
            Name = league.Name;
            SetsTillRank = league.SetsTillRank;
            DateCreated = league.DateCreated;
            AdminID = league.AdminID;
            ActiveSeasonID = league.ActiveSeasonID;
        }
    }
}