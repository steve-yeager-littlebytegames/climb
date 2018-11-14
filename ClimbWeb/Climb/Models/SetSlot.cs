namespace Climb.Models
{
    public class SetSlot
    {
        public enum Brackets
        {
            Winners,
            Losers,
            Grands,
        }

        public int Identifier { get; set; }
        public int TournamentID { get; set; }
        public string RoundName { get; set; }
        public int? WinSlotIdentifier { get; set; }
        public int? LoseSlotIdentifier { get; set; }
        public Brackets Bracket { get; set; }

        public Tournament Tournament { get; set; }
    }
}