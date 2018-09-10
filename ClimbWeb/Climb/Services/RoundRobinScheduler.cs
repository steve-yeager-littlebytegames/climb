using System;
using System.Collections.Generic;
using Climb.Models;
using MoreLinq.Extensions;

namespace Climb.Services
{
    public class RoundRobinScheduler : IScheduleFactory
    {
        private static readonly SeasonLeagueUser bye = new SeasonLeagueUser();

        public List<Set> GenerateSchedule(DateTime startDate, DateTime endDate, IReadOnlyList<SeasonLeagueUser> participants)
        {
            var participantsFull = GetParticipants(participants);
            var sets = new List<Set>(participantsFull.Count);

            var roundCount = participantsFull.Count - 1;
            var roundDays = (endDate - startDate).Days / roundCount;

            var nextDueDate = startDate;
            for(var i = 0; i < roundCount; i++)
            {
                nextDueDate = nextDueDate.AddDays(roundDays);
                var generatedSets = GenerateSets(participantsFull, nextDueDate);
                sets.AddRange(generatedSets);

                participantsFull.Insert(1, participantsFull[participantsFull.Count - 1]);
                participantsFull.RemoveAt(participantsFull.Count - 1);
            }

            return sets;
        }

        public void Reschedule(DateTime startDate, DateTime endDate, IReadOnlyList<Set> sets, IReadOnlyList<SeasonLeagueUser> participants)
        {
            var daysInRound = (endDate - startDate).Days / (participants.Count - 1);
            var dueDate = startDate;

            var scheduledSets = new HashSet<Set>(sets.Count);
            var playersInRound = new HashSet<int>();

            while(scheduledSets.Count != sets.Count)
            {
                dueDate = dueDate.AddDays(daysInRound);
                playersInRound.Clear();
                foreach(var set in sets)
                {
                    if(!scheduledSets.Contains(set) && !playersInRound.Contains(set.Player1ID) && !playersInRound.Contains(set.Player2ID))
                    {
                        set.DueDate = dueDate;

                        scheduledSets.Add(set);
                        playersInRound.Add(set.Player1ID);
                        playersInRound.Add(set.Player2ID);
                    }
                }
            }
        }

        private static IEnumerable<Set> GenerateSets(List<SeasonLeagueUser> participants, in DateTime dueDate)
        {
            var sets = new List<Set>(participants.Count / 2);

            var halfCount = participants.Count / 2;
            var firstHalf = participants.GetRange(0, halfCount);
            var secondHalf = participants.GetRange(halfCount, halfCount);
            secondHalf.Reverse();

            for(var i = 0; i < halfCount; i++)
            {
                var player1 = firstHalf[i];
                var player2 = secondHalf[i];
                if(player1 == bye || player2 == bye)
                {
                    continue;
                }

                var season = player1.Season;
                var set = new Set
                {
                    LeagueID = season.LeagueID,
                    SeasonID = season.ID,
                    DueDate = dueDate,
                    Player1ID = player1.LeagueUserID,
                    Player2ID = player2.LeagueUserID,
                    SeasonPlayer1ID = player1.ID,
                    SeasonPlayer2ID = player2.ID,
                };
                sets.Add(set);
            }

            return sets;
        }

        private static List<SeasonLeagueUser> GetParticipants(IEnumerable<SeasonLeagueUser> participants)
        {
            var participantsFull = new List<SeasonLeagueUser>(participants);
            if(participantsFull.Count % 2 == 1)
            {
                participantsFull.Add(bye);
            }

            participantsFull.Shuffle();
            return participantsFull;
        }
    }
}