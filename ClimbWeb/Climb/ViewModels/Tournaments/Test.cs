using Climb.Services;
using Newtonsoft.Json;

namespace Climb.ViewModels.Tournaments
{
    public class Test
    {
        public BracketGenerator.Tournament Tournament { get; }
        public string[][] Competitors { get; }
        public string BracketData { get; }

        public Test(BracketGenerator.Tournament tournament)
        {
            Tournament = tournament;
            Competitors = new string[tournament.Competitors.Count / 2][];

            int index = 0;
            for(var i = 0; i < tournament.Competitors.Count; i += 2)
            {
                Competitors[index] = new[] {GetPlayer(tournament.Competitors[i]), GetPlayer(tournament.Competitors[i + 1])};
                ++index;
            }

            var winnersBracket = new int[tournament.Winners.Count - 1][][];
            for(int i = 0; i < winnersBracket.Length; i++)
            {
                winnersBracket[i] = new int[tournament.Winners[i].Games.Count][];
                for(int j = 0; j < tournament.Winners[i].Games.Count; j++)
                {
                    //winnersBracket[i][j] = new int[0];
                    winnersBracket[i][j] = new[] {2, 1};
                }
            }

            var losersBracket = new int[tournament.Losers.Count][][];
            for(int i = 0; i < losersBracket.Length; i++)
            {
                losersBracket[i] = new int[tournament.Losers[i].Games.Count][];
                for(int j = 0; j < tournament.Losers[i].Games.Count; j++)
                {
                    //losersBracket[i][j] = new int[0];
                    losersBracket[i][j] = new[] {2, 1};
                }
            }

            var grandFinals = new[]
            {
                new []
                {
                    new[] {1, 2},
                },
                new []
                {
                    new[] {1, 2},
                }
            };

            var data = new
            {
                teams=Competitors,
                results= new [] {winnersBracket, losersBracket, grandFinals},
            };
            BracketData = JsonConvert.SerializeObject(data, Formatting.Indented);

            string GetPlayer(int? seed)
            {
                return seed == null ? null : $"Seed {seed + 1}";
            }
        }
    }
}