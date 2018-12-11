using System.Threading.Tasks;
using Climb.Data;

namespace Climb.Services.DataAnalyzers
{
    public class WinStreakAnalyzer : DataAnalyzer
    {
        public override Task<AnalyzerData> Analyze(int player1ID, int player2ID, ApplicationDbContext dbContext)
        {
            return Task.FromResult<AnalyzerData>(null);
        }
    }
}