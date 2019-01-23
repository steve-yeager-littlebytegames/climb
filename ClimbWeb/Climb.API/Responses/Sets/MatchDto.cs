using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Climb.Models;

namespace Climb.Responses.Sets
{
    public class MatchDto
    {
        public readonly int id;
        public readonly int index;
        public readonly int player1Score;
        public readonly int player2Score;
        [Required]
        public readonly IReadOnlyCollection<int> player1Characters;
        [Required]
        public readonly IReadOnlyCollection<int> player2Characters;
        public readonly int? stageID;

        public MatchDto(Match match, int player1ID)
        {
            id = match.ID;
            index = match.Index;
            player1Score = match.Player1Score;
            player2Score = match.Player2Score;
            stageID = match.StageID;

            var p1Chars = new List<int>();
            var p2Chars = new List<int>();
            foreach(var matchCharacter in match.MatchCharacters)
            {
                var charList = matchCharacter.LeagueUserID == player1ID ? p1Chars : p2Chars;
                charList.Add(matchCharacter.CharacterID);
            }

            player1Characters = p1Chars;
            player2Characters = p2Chars;
        }
    }
}