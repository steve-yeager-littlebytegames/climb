namespace Climb.Services
{
    public interface IBracketGenerator
    {
        int MinCompetitors { get; }
        BracketGenerator.TournamentData Generate(int competitorCount);
    }
}