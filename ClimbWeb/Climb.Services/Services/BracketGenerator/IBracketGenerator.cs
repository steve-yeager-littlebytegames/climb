namespace Climb.Services
{
    public interface IBracketGenerator
    {
        int MinCompetitors { get; }
        BracketGenerator.BracketData Generate(int competitorCount);
    }
}