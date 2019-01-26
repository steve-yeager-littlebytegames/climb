using System;
using JetBrains.Annotations;

namespace Climb.Models
{
    public class MatchCharacter
    {
        [UsedImplicitly]
        public int MatchID { get; private set; }
        [UsedImplicitly]
        public int CharacterID { get; private set; }
        [UsedImplicitly]
        public int LeagueUserID { get; private set; }
        [UsedImplicitly]
        public DateTime CreatedDate { get; private set; }

        [UsedImplicitly]
        public Match Match { get; private set; }
        [UsedImplicitly]
        public Character Character { get; private set; }
        [UsedImplicitly]
        public LeagueUser LeagueUser { get; private set; }

        public MatchCharacter()
        {
        }

        public MatchCharacter(int matchID, int characterID, int leagueUserID, DateTime createdDate)
        {
            MatchID = matchID;
            CharacterID = characterID;
            LeagueUserID = leagueUserID;
            CreatedDate = createdDate;
        }
    }
}