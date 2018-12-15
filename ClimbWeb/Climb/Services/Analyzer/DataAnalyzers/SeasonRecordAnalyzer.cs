using System.Collections.Generic;
using System.Threading.Tasks;
using Climb.Data;

namespace Climb.Services.DataAnalyzers
{
    public class SeasonRecordAnalyzer : DataAnalyzer
    {
        public override Task<IReadOnlyList<string>> Analyze(int player1ID, int player2ID, ApplicationDbContext dbContext)
        {
            return Task.FromResult<IReadOnlyList<string>>(null);
        }
    }
}