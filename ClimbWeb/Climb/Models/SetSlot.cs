namespace Climb.Models
{
    public class SetSlot
    {
        public int Identifier { get; set; }
        public int TournamentID { get; set; }
        public int RoundID { get; set; }
        public int? WinSlotIdentifier { get; set; }
        public int? LoseSlotIdentifier { get; set; }
        public int? SetID { get; set; }
        public bool IsBye { get; set; }
        public int? P1Game { get; set; }
        public int? P2Game { get; set; }
        
        // TODO: Do I need to store the players here? Or just leave that to the set? That would mean I have to delete the sets created when generating the bracket.

        public Tournament Tournament { get; set; }
        public Round Round { get; set; }
        public Set Set { get; set; }
    }
}