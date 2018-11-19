using Climb.Models;

namespace Climb.ViewModels.Tournaments
{
    public class GameViewModel
    {
        public int Identifier { get; set; }
        public bool IsBye { get; set; }
        public int? Player1ID { get; set; }
        public int? Player2ID { get; set; }
        public int? P1GameID { get; set; }
        public int? P2GameID { get; set; }
        public int P1Score { get; set; }
        public int P2Score { get; set; }

        public GameViewModel(SetSlot setSlot)
        {
            Identifier = setSlot.Identifier;
            IsBye = setSlot.IsBye;
        }
    }
}