using System;
using Newtonsoft.Json;

namespace Climb.Models
{
    public class MatchCharacter
    {
        public int MatchID { get; set; }
        public int CharacterID { get; set; }
        public int LeagueUserID { get; set; }
        public DateTime CreatedDate { get; set; }

        [JsonIgnore]
        public Match Match { get; set; }
        [JsonIgnore]
        public Character Character { get; set; }
        [JsonIgnore]
        public LeagueUser LeagueUser { get; set; }

        public MatchCharacter()
        {
        }

        public MatchCharacter(int matchID, int characterID, int leagueUserID)
        {
            MatchID = matchID;
            CharacterID = characterID;
            LeagueUserID = leagueUserID;
        }
    }
}