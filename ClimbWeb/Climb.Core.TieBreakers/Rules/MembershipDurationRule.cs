using System;
using System.Collections.Generic;

namespace Climb.Core.TieBreakers
{
    internal class MembershipDurationRule : TieBreakerRule
    {
        private readonly DateTime now;

        public MembershipDurationRule(DateTime now)
        {
            this.now = now;
        }

        protected override int GetParticipantScore((IParticipant participant, ParticipantRecord record) participant, IReadOnlyDictionary<IParticipant, ParticipantRecord> tiedParticipants)
        {
            return (int)(now - participant.record.LeagueJoinDate).TotalSeconds;
        }
    }
}