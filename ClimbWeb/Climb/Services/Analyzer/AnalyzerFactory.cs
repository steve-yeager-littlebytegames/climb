using System.Collections.Generic;
using Climb.Services.DataAnalyzers;

namespace Climb.Services
{
    public class AnalyzerFactory : IAnalyzerFactory
    {
<<<<<<< master
        private readonly IDateService dateService;

        public AnalyzerFactory(IDateService dateService)
        {
            this.dateService = dateService;
        }

=======
>>>>>>> Implementing AnalyzerService
        public IReadOnlyList<DataAnalyzer> CreateAnalyzers()
        {
            return new List<DataAnalyzer>
            {
<<<<<<< master
                new LeagueRecordAnalyzer(dateService),
=======
                new LeagueRecordAnalyzer(),
>>>>>>> Implementing AnalyzerService
                new SeasonRecordAnalyzer(),
                new WinStreakAnalyzer(),
            };
        }
    }
}