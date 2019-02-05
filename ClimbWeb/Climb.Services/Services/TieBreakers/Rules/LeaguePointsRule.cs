using System.Collections.Generic;

namespace Climb.Core.TieBreakers
{
    public class LeaguePointsRule : TieBreakerRule
    {
        protected override int GetParticipantScore((IParticipant participant, ParticipantRecord record) participant, IReadOnlyDictionary<IParticipant, ParticipantRecord> tiedParticipants)
        {
            return participant.record.LeaguePoints;
        }
    }
}