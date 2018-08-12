using System;
using System.ComponentModel.DataAnnotations;
using Climb.Models;

namespace Climb.Responses.Sets
{
    public class SetDto
    {
        public int ID { get; }
        public int LeagueID { get; }
        public string LeagueName { get; }
        public int? SeasonID { get; }
        public int? SeasonIndex { get; }
        public int GameID { get; }
        public int Player1ID { get; }
        public int Player2ID { get; }
        public int? Player1Score { get; }
        public int? Player2Score { get; }
        public DateTime DueDate { get; }
        public DateTime? UpdatedDate { get; }
        [Required]
        public MatchDto[] Matches { get; }
        public bool IsLocked { get; set; }
        public bool IsComplete { get; set; }

        private SetDto(Set set, MatchDto[] matches, int gameID)
        {
            ID = set.ID;
            LeagueID = set.LeagueID;
            LeagueName = set.League.Name;
            SeasonID = set.SeasonID;
            SeasonIndex = set.Season?.Index;
            Player1ID = set.Player1ID;
            Player2ID = set.Player2ID;
            Player1Score = set.Player1Score;
            Player2Score = set.Player2Score;
            DueDate = set.DueDate;
            UpdatedDate = set.UpdatedDate;
            Matches = matches;
            GameID = gameID;
        }

        public static SetDto Create(Set set, int gameID)
        {
            var matches = new MatchDto[set.Matches.Count];
            for(var i = 0; i < matches.Length; i++)
            {
                matches[i] = new MatchDto(set.Matches[i], set.Player1ID);
            }

            return new SetDto(set, matches, gameID);
        }
    }
}