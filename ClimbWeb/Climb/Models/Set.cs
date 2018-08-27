using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace Climb.Models
{
    public class Set
    {
        private const int ForfeitScore = -1;

        public int ID { get; set; }
        public int LeagueID { get; set; }
        public int? SeasonID { get; set; }
        public int Player1ID { get; set; }
        public int Player2ID { get; set; }
        public int? SeasonPlayer1ID { get; set; }
        public int? SeasonPlayer2ID { get; set; }
        public int? Player1Score { get; set; }
        public int? Player2Score { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool IsLocked { get; set; }
        public bool IsComplete { get; set; }
        public bool IsForfeit { get; set; }
        public int Player1SeasonPoints { get; set; }
        public int Player2SeasonPoints { get; set; }

        [JsonIgnore]
        public League League { get; set; }
        [JsonIgnore]
        public Season Season { get; set; }
        [JsonIgnore]
        [InverseProperty("P1Sets")]
        [ForeignKey("Player1ID")]
        public LeagueUser Player1 { get; set; }
        [JsonIgnore]
        [InverseProperty("P2Sets")]
        [ForeignKey("Player2ID")]
        public LeagueUser Player2 { get; set; }
        [JsonIgnore]
        [InverseProperty("P1Sets")]
        [ForeignKey("SeasonPlayer1ID")]
        public SeasonLeagueUser SeasonPlayer1 { get; set; }
        [JsonIgnore]
        [InverseProperty("P2Sets")]
        [ForeignKey("SeasonPlayer2ID")]
        public SeasonLeagueUser SeasonPlayer2 { get; set; }

        [Required]
        public List<Match> Matches { get; set; }
        [JsonIgnore]
        public int? WinnerID => Player1Score > Player2Score ? Player1ID : Player1Score < Player2Score ? (int?)Player2ID : null;
        [JsonIgnore]
        public int? LoserID => Player1Score > Player2Score ? Player2ID : Player1Score < Player2Score ? (int?)Player1ID : null;
        public int? SeasonWinnerID => Player1Score > Player2Score ? SeasonPlayer1ID : Player1Score < Player2Score ? SeasonPlayer2ID : null;
        public int? SeasonLoserID => Player1Score > Player2Score ? SeasonPlayer2ID : Player1Score < Player2Score ? SeasonPlayer1ID : null;

        public Set()
        {
        }

        public bool IsPlaying(int leagueUserID)
        {
            return Player1ID == leagueUserID || Player2ID == leagueUserID;
        }

        public int GetOpponentID(int leagueUserID)
        {
            if(Player1ID == leagueUserID)
            {
                return Player2ID;
            }

            if(Player2ID == leagueUserID)
            {
                return Player1ID;
            }

            throw new ArgumentException($"LeagueUser with ID '{leagueUserID}' is not playing this set.");
        }

        public void Forfeit(int leagueUserID)
        {
            if(Player1ID == leagueUserID)
            {
                Player1Score = ForfeitScore;
                Player2Score = 0;
            }
            else if(Player2ID == leagueUserID)
            {
                Player1Score = 0;
                Player2Score = ForfeitScore;
            }
            else
            {
                throw new ArgumentException($"League User '{leagueUserID}' is not in Set '{ID}'.", nameof(leagueUserID));
            }

            IsForfeit = true;
            IsComplete = true;
        }
    }
}