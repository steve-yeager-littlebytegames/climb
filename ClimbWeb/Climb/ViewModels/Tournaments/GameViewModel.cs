using Climb.Models;

namespace Climb.ViewModels.Tournaments
{
    public class GameViewModel
    {
        public enum Player
        {
            None,
            P1,
            P2,
        }

        public int Identifier { get; }
        public bool IsBye { get; }
        public TournamentUser Player1 { get; }
        public TournamentUser Player2 { get; }
        public int? P1GameID { get; }
        public int? P2GameID { get; }
        public string P1Score { get; }
        public string P2Score { get; }
        public int? WinID { get; }
        public Player Winner { get; }
        public int? SetID { get; }

        private GameViewModel(SetSlot setSlot)
        {
            const string noScoreLabel = "-";

            Identifier = setSlot.Identifier;
            IsBye = setSlot.IsBye;
            WinID = setSlot.WinSlotIdentifier;
            Player1 = setSlot.User1;
            Player2 = setSlot.User2;
            P1GameID = setSlot.P1Game;
            P2GameID = setSlot.P2Game;

            int? p1Score = setSlot.Set?.Player1Score;
            int? p2Score = setSlot.Set?.Player2Score;
            P1Score = p1Score?.ToString() ?? noScoreLabel;
            P2Score = p2Score?.ToString() ?? noScoreLabel;
            Winner = p1Score > p2Score ? Player.P1 : p2Score > p1Score ? Player.P2 : Player.None;

            SetID = setSlot.Set?.ID;
        }

        public static GameViewModel Create(SetSlot setSlot) => new GameViewModel(setSlot);
    }
}