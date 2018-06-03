﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        public bool HasLeft { get; set; }
        public int Points { get; set; }
        public int Rank { get; set; }
        // TODO: Update this value.
        public int SetCount { get; set; }

        [JsonIgnore]
        public League League { get; set; }
        [JsonIgnore]
        public ApplicationUser User { get; set; }
        [JsonIgnore]
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
            return Points.CompareTo(other.Points);
        }
    }
}