using System.Collections.Generic;

namespace Climb.Core.TieBreakers
{
    public interface ITieBreaker
    {
        void Break(IReadOnlyDictionary<IParticipant, ParticipantRecord> participants);
    }
}