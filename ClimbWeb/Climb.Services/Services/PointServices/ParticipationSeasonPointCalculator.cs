using Climb.Models;

namespace Climb.Services
{
    public class ParticipationSeasonPointCalculator : ISeasonPointCalculator
    {
        public (int winnerPointDelta, int loserPointDelta) CalculatePointDeltas(SeasonLeagueUser winner, SeasonLeagueUser loser)
        {
            return (2, 1);
        }
    }
}