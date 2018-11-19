using Climb.Services;

namespace Climb.ViewModels.Tournaments
{
    public class Test
    {
        public BracketGenerator.TournamentData Tournament { get; }

        public Test(BracketGenerator.TournamentData tournament)
        {
            Tournament = tournament;

            var winnersBracket = new int[tournament.Winners.Count - 1][][];
            for(var i = 0; i < winnersBracket.Length; i++)
            {
                winnersBracket[i] = new int[tournament.Winners[i].Games.Count][];
                for(var j = 0; j < tournament.Winners[i].Games.Count; j++)
                {
                    //winnersBracket[i][j] = new int[0];
                    winnersBracket[i][j] = new[] {2, 1};
                }
            }

            var losersBracket = new int[tournament.Losers.Count][][];
            for(var i = 0; i < losersBracket.Length; i++)
            {
                losersBracket[i] = new int[tournament.Losers[i].Games.Count][];
                for(var j = 0; j < tournament.Losers[i].Games.Count; j++)
                {
                    //losersBracket[i][j] = new int[0];
                    losersBracket[i][j] = new[] {2, 1};
                }
            }
        }
    }
}