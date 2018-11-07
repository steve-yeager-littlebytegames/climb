namespace Climb.Models
{
    public class SetSlot
    {
        public int ID { get; set; }
        public int TournamentID { get; set; }
        public string RoundName { get; set; }
        public int? WinSlotID { get; set; }
        public int? LoseSlotID { get; set; }

        public Tournament Tournament { get; set; }
    }
}