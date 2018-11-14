using System.Collections.Generic;

namespace Climb.Services
{
    public partial class BracketGenerator
    {
        public class TournamentData
        {
            public int GameCount { get; set; }
            public int RoundCount { get; set; }
            public List<int?> Competitors { get; }

            public List<RoundData> Winners { get; } = new List<RoundData>();
            public List<RoundData> Losers { get; } = new List<RoundData>();
            public RoundData GrandFinals { get; set; }

            public TournamentData(List<int?> competitors)
            {
                Competitors = competitors;
            }

            public RoundData AddRound(List<RoundData> bracket)
            {
                ++RoundCount;
                var round = new RoundData(RoundCount);
                bracket.Add(round);
                return round;
            }

            public GameData AddGame(RoundData round, int? p1 = null, int? p2 = null, bool isBye = false)
            {
                ++GameCount;
                var game = new GameData(GameCount, isBye, p1, p2);
                round.Games.Add(game);
                return game;
            }
        }
    }
}