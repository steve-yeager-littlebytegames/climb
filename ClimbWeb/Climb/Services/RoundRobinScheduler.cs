using System;
using System.Collections.Generic;
using Climb.Models;
using MoreLinq.Extensions;

namespace Climb.Services
{
    public class RoundRobinScheduler : ScheduleFactory
    {
        private static readonly SeasonLeagueUser bye = new SeasonLeagueUser();

        protected override List<Set> GenerateScheduleInternal(Season season)
        {
            var participants = GetParticipants(season);
            var sets = new List<Set>(season.Participants.Count);

            var roundCount = participants.Count - 1;
            var roundDays = (season.EndDate - season.StartDate).Days / roundCount;

            var nextDueDate = season.StartDate;
            for(var i = 0; i < roundCount; i++)
            {
                nextDueDate = nextDueDate.AddDays(roundDays);
                var generatedSets = GenerateSets(participants, nextDueDate);
                sets.AddRange(generatedSets);

                participants.Insert(1, participants[participants.Count - 1]);
                participants.RemoveAt(participants.Count - 1);
            }

            return sets;
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

        private static List<SeasonLeagueUser> GetParticipants(Season season)
        {
            var participants = new List<SeasonLeagueUser>(season.Participants);
            if(participants.Count % 2 == 1)
            {
                participants.Add(bye);
            }

            participants.Shuffle();
            return participants;
        }
    }
}