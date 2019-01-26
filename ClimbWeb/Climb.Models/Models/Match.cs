using System.Collections.Generic;
using JetBrains.Annotations;

namespace Climb.Models
{
    public class Match
    {
        [UsedImplicitly]
        public int ID { get; private set; }
        [UsedImplicitly]
        public int SetID { get; set; }
        [UsedImplicitly]
        public int Index { get; set; }
        [UsedImplicitly]
        public int Player1Score { get; set; }
        [UsedImplicitly]
        public int Player2Score { get; set; }
        [UsedImplicitly]
        public int? StageID { get; set; }

        [UsedImplicitly]
        public Set Set { get; set; }
        [UsedImplicitly]
        public Stage Stage { get; set; }
        [UsedImplicitly]
        public HashSet<MatchCharacter> MatchCharacters { get; set; }

        public Match()
        {
        }

        public Match(int setID, int index, int player1Score, int player2Score, int? stageID = null)
        {
            SetID = setID;
            Index = index;
            Player1Score = player1Score;
            Player2Score = player2Score;
            StageID = stageID;
            MatchCharacters = new HashSet<MatchCharacter>();
        }
    }
}