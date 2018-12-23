using System.Collections.Generic;

namespace Climb.Core.TieBreakers
{
    public abstract class TieBreakerRule
    {
        protected abstract int GetParticipantScore((IParticipant participant, ParticipantRecord record) participant, IReadOnlyDictionary<IParticipant, ParticipantRecord> tiedParticipants);

        public List<(IParticipant participant, int score)> Evaluate(IReadOnlyDictionary<IParticipant, ParticipantRecord> tiedParticipants)
        {
            var scores = new List<(IParticipant participant, int score)>();

            foreach(var participant in tiedParticipants)
            {
                var score = GetParticipantScore((participant.Key, participant.Value), tiedParticipants);
                scores.Add((participant.Key, score));
            }

            return scores;
        }
    }
}