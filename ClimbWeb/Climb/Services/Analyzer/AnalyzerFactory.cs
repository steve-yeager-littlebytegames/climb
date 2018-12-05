using System.Collections.Generic;
using Climb.Services.DataAnalyzers;

namespace Climb.Services
{
    public class AnalyzerFactory : IAnalyzerFactory
    {
        public IReadOnlyList<DataAnalyzer> CreateAnalyzers()
        {
            return new List<DataAnalyzer>
            {
                new LeagueRecordAnalyzer(),
                new SeasonRecordAnalyzer(),
                new WinStreakAnalyzer(),
            };
        }
    }
}