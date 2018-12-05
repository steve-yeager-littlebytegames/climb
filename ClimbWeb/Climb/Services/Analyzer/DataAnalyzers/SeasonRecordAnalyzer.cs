<<<<<<< master
﻿using System.Threading.Tasks;
using Climb.Data;
=======
﻿using Climb.Data;
>>>>>>> Implementing AnalyzerService

namespace Climb.Services.DataAnalyzers
{
    public class SeasonRecordAnalyzer : DataAnalyzer
    {
<<<<<<< master
        public override Task<AnalyzerData> Analyze(int player1ID, int player2ID, ApplicationDbContext dbContext)
        {
            return Task.FromResult<AnalyzerData>(null);
=======
        public override AnalyzerData Analyze(int player1ID, int player2ID, ApplicationDbContext dbContext)
        {
            throw new System.NotImplementedException();
>>>>>>> Implementing AnalyzerService
        }
    }
}