using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Climb.Data;
using Newtonsoft.Json;

namespace Climb.Models
{
    public class LeagueUser : IComparable<LeagueUser>
    {
        public int ID { get; set; }
        public int LeagueID { get; set; }
        [Required]
        public string UserID { get; set; }
        public string DisplayName { get; set; }
        public bool HasLeft { get; set; }
        public int Points { get; set; }
        public int Rank { get; set; }
        public int SetCount { get; set; }
        public DateTime JoinDate { get; set; }
        public bool IsNewcomer { get; set; } = true;

        [JsonIgnore]
        public League League { get; set; }
        [JsonIgnore]
        public ApplicationUser User { get; set; }
        [JsonIgnore]
        [InverseProperty("LeagueUser")]
        public HashSet<SeasonLeagueUser> Seasons { get; set; }
        [JsonIgnore]
        public List<RankSnapshot> RankSnapshots { get; set; }

        #region For DB
        [JsonIgnore]
        public HashSet<Set> P1Sets { get; set; }
        [JsonIgnore]
        public HashSet<Set> P2Sets { get; set; }
        #endregion

        public LeagueUser()
        {
        }

        public LeagueUser(int leagueID, string userID)
        {
            LeagueID = leagueID;
            UserID = userID;
        }

        public int CompareTo(LeagueUser other)
        {
            return other.Points.CompareTo(Points);
        }
    }
}