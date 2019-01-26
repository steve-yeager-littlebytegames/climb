using System.Collections.Generic;

namespace Climb.Models
{
    public class Round
    {
        public enum Brackets
        {
            Winners,
            Losers,
            Grands,
        }

        public int ID { get; set; }
        public int Index { get; set; }
        public string Name { get; set; }
        public int TournamentID { get; set; }
        public Brackets Bracket { get; set; }

        public Tournament Tournament { get; set; }
        public List<SetSlot> SetSlots { get; set; }
    }
}