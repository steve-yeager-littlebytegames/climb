using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Climb.Data;
using Newtonsoft.Json;

namespace Climb.Models
{
    public class League
    {
        public const int StartingPoints = 2000;

        public int ID { get; set; }
        public int GameID { get; set; }
        public int? OrganizationID { get; set; }
        [Required]
        public string Name { get; set; } = "";
        public int SetsTillRank { get; set; } = 4;
        public DateTime DateCreated { get; set; }
        public string AdminID { get; set; }
        public int? ActiveSeasonID { get; set; }

        [JsonIgnore]
        public Game Game { get; set; }
        [JsonIgnore]
        public List<LeagueUser> Members { get; set; }
        [JsonIgnore]
        [InverseProperty(nameof(Season.League))]
        public HashSet<Season> Seasons { get; set; }
        [JsonIgnore]
        public List<Set> Sets { get; set; }
        public ApplicationUser Admin { get; set; }
        public Organization Organization { get; set; }
        [JsonIgnore]
        public Season ActiveSeason { get; set; }

        public League()
        {
        }

        public League(int gameID, string name, string adminID)
        {
            GameID = gameID;
            Name = name;
            AdminID = adminID;
            DateCreated = DateTime.Today;
        }

        public bool IsMemberNew(LeagueUser member) => member.SetCount < SetsTillRank;
    }
}