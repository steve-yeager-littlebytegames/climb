using System;
using System.Collections.Generic;

namespace Climb.Core.TieBreakers
{
    public class ParticipantRecord
    {
        private readonly Dictionary<int, int> wins = new Dictionary<int, int>();

        public int LeaguePoints { get; }
        public DateTime LeagueJoinDate { get; }
        public int TotalWins { get; private set; }
        public IReadOnlyDictionary<int, int> Wins => wins;

        public ParticipantRecord(int leaguePoints, DateTime leagueJoinDate)
        {
            LeaguePoints = leaguePoints;
            LeagueJoinDate = leagueJoinDate;
        }

        public void AddWins(IReadOnlyList<int> opponentIDs)
        {
            TotalWins += opponentIDs.Count;

            foreach(var opponentID in opponentIDs)
            {
                if(wins.ContainsKey(opponentID))
                {
                    ++wins[opponentID];
                }
                else
                {
                    wins.Add(opponentID, 1);
                }
            }
        }

        public int TimesBeatenOpponent(int opponentID)
        {
            wins.TryGetValue(opponentID, out var winCount);
            return winCount;
        }
    }
}