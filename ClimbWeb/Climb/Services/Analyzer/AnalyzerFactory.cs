using System.Collections.Generic;
using Climb.Services.DataAnalyzers;

namespace Climb.Services
{
    public class AnalyzerFactory : IAnalyzerFactory
    {
        private readonly IDateService dateService;

        public AnalyzerFactory(IDateService dateService)
        {
            this.dateService = dateService;
        }

        public IReadOnlyList<DataAnalyzer> CreateAnalyzers()
        {
            return new List<DataAnalyzer>
            {
                new LeagueRecordAnalyzer(dateService),
                new SeasonRecordAnalyzer(),
                new WinStreakAnalyzer(),
            };
        }
    }
}