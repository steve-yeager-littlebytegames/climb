using System.Collections.Generic;
using Climb.Services.DataAnalyzers;

namespace Climb.Services
{
    public class AnalyzerFactory : IAnalyzerFactory
    {
<<<<<<< master
<<<<<<< master
=======
>>>>>>> Send data to WPF app
        private readonly IDateService dateService;

        public AnalyzerFactory(IDateService dateService)
        {
            this.dateService = dateService;
        }

<<<<<<< master
=======
>>>>>>> Implementing AnalyzerService
=======
>>>>>>> Send data to WPF app
        public IReadOnlyList<DataAnalyzer> CreateAnalyzers()
        {
            return new List<DataAnalyzer>
            {
<<<<<<< master
<<<<<<< master
                new LeagueRecordAnalyzer(dateService),
=======
                new LeagueRecordAnalyzer(),
>>>>>>> Implementing AnalyzerService
=======
                new LeagueRecordAnalyzer(dateService),
>>>>>>> Send data to WPF app
                new SeasonRecordAnalyzer(),
                new WinStreakAnalyzer(),
            };
        }
    }
}