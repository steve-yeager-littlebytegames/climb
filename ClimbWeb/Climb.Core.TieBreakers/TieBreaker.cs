using System;
using System.Collections.Generic;
using System.Linq;

namespace Climb.Core.TieBreakers
{
    public class TieBreaker : ITieBreaker
    {
        private readonly List<TieBreakerRule> rules = new List<TieBreakerRule>();

        internal TieBreaker AddRule(TieBreakerRule rule)
        {
            if(rule == null)
            {
                throw new ArgumentNullException(nameof(rule));
            }

            if(rules.Any(a => a.GetType() == rule.GetType()))
            {
                throw new ArgumentException(nameof(rule));
            }

            rules.Add(rule);

            return this;
        }

        public void Break(IReadOnlyDictionary<IParticipant, ParticipantRecord> participants)
        {
            var round = rules.Count;
            foreach(var tiebreakAttempt in rules)
            {
                var userScores = tiebreakAttempt.Evaluate(participants).OrderBy(p => p.score);

                var lastScore = int.MinValue;
                var place = 0;
                foreach(var (participant, score) in userScores)
                {
                    if(score != lastScore)
                    {
                        lastScore = score;
                        ++place;
                    }

                    var roundScore = Math.Pow(place * 2, round);
                    participant.TieBreakerPoints += (int)roundScore;
                }

                --round;
            }
        }
    }
}