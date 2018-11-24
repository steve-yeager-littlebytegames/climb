using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Climb.Data;
using JetBrains.Annotations;

namespace Climb.Models
{
    public class League
    {
        public const int StartingPoints = 2000;

        [UsedImplicitly]
        public int ID { get; private set; }
        public int GameID { get; set; }
        [UsedImplicitly]
        public int? OrganizationID { get; private set; }
        public string Name { get; set; } = "";
        public int SetsTillRank { get; set; } = 4;
        [UsedImplicitly]
        public DateTime DateCreated { get; private set; }
        public string AdminID { get; set; }
        public int? ActiveSeasonID { get; set; }
        public DateTime LastRankUpdate { get; set; }

        [UsedImplicitly]
        public Game Game { get; set; }
        [UsedImplicitly]
        public List<LeagueUser> Members { get; set; }
        [UsedImplicitly]
        [InverseProperty(nameof(Season.League))]
        public HashSet<Season> Seasons { get; set; }
        [UsedImplicitly]
        public List<Set> Sets { get; set; }
        [UsedImplicitly]
        public ApplicationUser Admin { get; set; }
        [UsedImplicitly]
        public Organization Organization { get; set; }
        [UsedImplicitly]
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

        public override string ToString() => $"{ID}:{Name}";

        public bool IsMemberNew(LeagueUser member) => member.SetCount < SetsTillRank;
    }
}