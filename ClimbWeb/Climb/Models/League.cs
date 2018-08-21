using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Climb.Data;
using Newtonsoft.Json;

namespace Climb.Models
{
    public class League
    {
        public int ID { get; set; }
        public int GameID { get; set; }
        public int? OrganizationID { get; set; }
        [Required]
        public string Name { get; set; } = "";
        public int SetsTillRank { get; set; } = 4;
        public DateTime DateCreated { get; set; }
        public string AdminID { get; set; }

        [JsonIgnore]
        public Game Game { get; set; }
        [JsonIgnore]
        public List<LeagueUser> Members { get; set; }
        [JsonIgnore]
        public HashSet<Season> Seasons { get; set; }
        [JsonIgnore]
        public List<Set> Sets { get; set; }
        public ApplicationUser Admin { get; set; }
        public Organization Organization { get; set; }

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