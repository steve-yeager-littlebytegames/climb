using System;
using System.Collections.Generic;

namespace Climb.Core.TieBreakers.Test
{
    public class ParticipantsBuilder
    {
        private readonly Dictionary<IParticipant, ParticipantRecord> participants = new Dictionary<IParticipant, ParticipantRecord>();
        private ParticipantRecord lastRecord;

        public ParticipantsBuilder Add(int id = 0, int leaguePoints = 0, DateTime joinDate = default)
        {
            lastRecord = new ParticipantRecord(leaguePoints, joinDate);
            participants.Add(new FakeParticipant(id), lastRecord);
            return this;
        }

        public ParticipantsBuilder Edit(Action<ParticipantRecord> record)
        {
            record(lastRecord);
            return this;
        }

        public static ParticipantsBuilder Create()
        {
            return new ParticipantsBuilder();
        }

        public static implicit operator Dictionary<IParticipant, ParticipantRecord>(ParticipantsBuilder builder)
        {
            return builder.participants;
        }
    }
}