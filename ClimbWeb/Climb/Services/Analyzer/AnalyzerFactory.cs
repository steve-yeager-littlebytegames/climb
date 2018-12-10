using System.Collections.Generic;
using Climb.Services.DataAnalyzers;

namespace Climb.Services
{
    public class AnalyzerFactory : IAnalyzerFactory
    {
<<<<<<< HEAD
<<<<<<< master
<<<<<<< master
=======
>>>>>>> Send data to WPF app
=======
>>>>>>> data-analyzer
        private readonly IDateService dateService;

        public AnalyzerFactory(IDateService dateService)
        {
            this.dateService = dateService;
        }

<<<<<<< HEAD
<<<<<<< master
=======
>>>>>>> Implementing AnalyzerService
=======
>>>>>>> Send data to WPF app
=======
>>>>>>> data-analyzer
        public IReadOnlyList<DataAnalyzer> CreateAnalyzers()
        {
            return new List<DataAnalyzer>
            {
<<<<<<< HEAD
<<<<<<< master
<<<<<<< master
                new LeagueRecordAnalyzer(dateService),
=======
                new LeagueRecordAnalyzer(),
>>>>>>> Implementing AnalyzerService
=======
                new LeagueRecordAnalyzer(dateService),
>>>>>>> Send data to WPF app
=======
                new LeagueRecordAnalyzer(dateService),
>>>>>>> data-analyzer
                new SeasonRecordAnalyzer(),
                new WinStreakAnalyzer(),
            };
        }
    }
}