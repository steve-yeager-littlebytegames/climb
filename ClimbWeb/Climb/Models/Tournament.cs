using System.Collections.Generic;

namespace Climb.Models
{
    public class Tournament
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int LeagueID { get; set; }
        public int? SeasonID { get; set; }

        public League League { get; set; }
        public Season Season { get; set; }
        public LeagueUser[] LeagueUsers { get; set; }
        public SetSlot[] SetSlots { get; set; }
        public Set[] Sets { get; set; }
        public List<TournamentUser> TournamentUsers { get; set; }
    }
}