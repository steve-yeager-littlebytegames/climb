using System;

namespace Climb.Models
{
    public class SetSlot
    {
        public int ID { get; set; }
        public int Identifier { get; set; }
        public int TournamentID { get; set; }
        public int RoundID { get; set; }
        public int? WinSlotIdentifier { get; set; }
        public int? LoseSlotIdentifier { get; set; }
        public int? P1Game { get; set; }
        public int? P2Game { get; set; }
        public int? User1ID { get; set; }
        public int? User2ID { get; set; }
        public bool IsBye { get; set; }

        public Tournament Tournament { get; set; }
        public Round Round { get; set; }
        public Set Set { get; set; }
        public TournamentUser User1 { get; set; }
        public TournamentUser User2 { get; set; }

        public bool IsFull => User1ID != null && User2ID != null;

        public void AssignPlayer(SetSlot fromSlot, int tournamentUserID)
        {
            if(P1Game == fromSlot.Identifier)
            {
                User1ID = tournamentUserID;
            }
            else if(P2Game == fromSlot.Identifier)
            {
                User2ID = tournamentUserID;
            }
            else
            {
                throw new ArgumentException($"Slot {fromSlot.Identifier} is not a child of slot {Identifier}.");
            }
        }
    }
}