using Climb.Models;

namespace Climb.Services
{
    public interface IBracketGenerator
    {
        int MinCompetitors { get; }
        TournamentData Generate(int competitorCount);
    }
}