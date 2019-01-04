using Climb.Models;

namespace Climb.Services
{
    public interface ISeasonPointCalculator
    {
        (int winnerPointDelta, int loserPointDelta) CalculatePointDeltas(SeasonLeagueUser winner, SeasonLeagueUser loser);
    }
}