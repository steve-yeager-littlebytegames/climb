using System.Collections.Generic;

namespace Climb.Services
{
    public partial class BracketGenerator
    {
        public class BracketData
        {
            public int GameCount { get; private set; }
            public int RoundCount { get; set; }

            public List<RoundData> Winners { get; } = new List<RoundData>();
            public List<RoundData> Losers { get; } = new List<RoundData>();
            public List<RoundData> GrandFinals { get; set; }

            public RoundData AddRound(List<RoundData> bracket)
            {
                ++RoundCount;
                var round = new RoundData(bracket.Count);
                bracket.Add(round);
                return round;
            }

            public GameData AddGame(RoundData round, int? p1 = null, int? p2 = null, bool isBye = false)
            {
                ++GameCount;
                var game = new GameData(GameCount, p1, p2);
                round.Games.Add(game);
                return game;
            }
        }
    }
}