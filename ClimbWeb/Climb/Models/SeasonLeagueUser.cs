using System;
using System.Collections.Generic;
using Climb.Core.TieBreakers;
using Climb.Data;

namespace Climb.Models
{
    public class SeasonLeagueUser : IComparable<SeasonLeagueUser>, IParticipant
    {
        public int ID { get; set; }
        public int SeasonID { get; set; }
        public int LeagueUserID { get; set; }
        public string UserID { get; set; }
        public int Standing { get; set; }
        public int Points { get; set; }
        public int TieBreakerPoints { get; set; }
        public bool HasLeft { get; set; }

        public Season Season { get; set; }
        public LeagueUser LeagueUser { get; set; }
        public ApplicationUser User { get; set; }
        public List<Set> P1Sets { get; set; }
        public List<Set> P2Sets { get; set; }

        public SeasonLeagueUser()
        {
        }

        public SeasonLeagueUser(int seasonID, int leagueUserID, string userID)
        {
            SeasonID = seasonID;
            LeagueUserID = leagueUserID;
            UserID = userID;
        }

        public int CompareTo(SeasonLeagueUser other)
        {
            return Points != other.Points ? Points.CompareTo(other.Points) : TieBreakerPoints.CompareTo(other.TieBreakerPoints);
        }
    }
}