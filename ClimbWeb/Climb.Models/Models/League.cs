using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Climb.Models
{
    public class League
    {
        public const int StartingPoints = 2000;

        public int ID { get; set; }
        public int GameID { get; set; }
        public int? OrganizationID { get; set; }
        public string Name { get; set; } = "";
        public int SetsTillRank { get; set; } = 4;
        public DateTime DateCreated { get; set; }
        public string AdminID { get; set; }
        public int? ActiveSeasonID { get; set; }
        public DateTime LastRankUpdate { get; set; }

        public Game Game { get; set; }
        public List<LeagueUser> Members { get; set; }
        [InverseProperty(nameof(Season.League))]
        public HashSet<Season> Seasons { get; set; }
        public List<Set> Sets { get; set; }
        public ApplicationUser Admin { get; set; }
        public Organization Organization { get; set; }
        public Season ActiveSeason { get; set; }

        public League()
        {
        }

        public League(int gameID, string name, string adminID, DateTime dateCreated)
        {
            GameID = gameID;
            Name = name;
            AdminID = adminID;
            DateCreated = dateCreated;
        }

        public bool IsMemberNew(LeagueUser member) => member.SetCount < SetsTillRank;

        public override string ToString() => $"{ID}:{Name}";
    }
}