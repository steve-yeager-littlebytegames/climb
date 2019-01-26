using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        public RankTrends RankTrend { get; set; }

        public League League { get; set; }
        public ApplicationUser User { get; set; }
        [InverseProperty("LeagueUser")]
        public HashSet<SeasonLeagueUser> Seasons { get; set; }
        public List<RankSnapshot> RankSnapshots { get; set; }
        public HashSet<Set> P1Sets { get; set; }
        public HashSet<Set> P2Sets { get; set; }

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