using Climb.Models;

namespace Climb.ViewModels.Tournaments
{
    public class GameViewModel
    {
        public int Identifier { get; set; }
        public bool IsBye { get; set; }
        public TournamentUser Player1 { get; set; }
        public TournamentUser Player2 { get; set; }
        public int? P1GameID { get; set; }
        public int? P2GameID { get; set; }
        public int P1Score { get; set; }
        public int P2Score { get; set; }
        public int? WinID { get; }

        private GameViewModel(SetSlot setSlot)
        {
            Identifier = setSlot.Identifier;
            IsBye = setSlot.IsBye;
            WinID = setSlot.WinSlotIdentifier;
            Player1 = setSlot.User1;
            Player2 = setSlot.User2;
            P1GameID = setSlot.P1Game;
            P2GameID = setSlot.P2Game;
            P1Score = setSlot.Set?.Player1Score ?? 0;
            P2Score = setSlot.Set?.Player2Score ?? 0;
        }

        public static GameViewModel Create(SetSlot setSlot) => new GameViewModel(setSlot);
    }
}