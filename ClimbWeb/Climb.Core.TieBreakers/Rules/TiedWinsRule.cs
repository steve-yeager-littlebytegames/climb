using System.Collections.Generic;
using System.Linq;

namespace Climb.Core.TieBreakers
{
    internal class TiedWinsRule : TieBreakerRule
    {
        protected override int GetParticipantScore((IParticipant participant, ParticipantRecord record) participant, IReadOnlyDictionary<IParticipant, ParticipantRecord> tiedParticipants)
        {
            return tiedParticipants.Sum(tp => participant.record.TimesBeatenOpponent(tp.Key.ID));
        }
    }
}