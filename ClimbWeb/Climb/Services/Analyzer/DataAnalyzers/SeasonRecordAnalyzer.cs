<<<<<<< master
<<<<<<< master
﻿using System.Threading.Tasks;
using Climb.Data;
=======
﻿using Climb.Data;
>>>>>>> Implementing AnalyzerService
=======
﻿using System.Threading.Tasks;
using Climb.Data;
>>>>>>> Send data to WPF app

namespace Climb.Services.DataAnalyzers
{
    public class SeasonRecordAnalyzer : DataAnalyzer
    {
<<<<<<< master
<<<<<<< master
        public override Task<AnalyzerData> Analyze(int player1ID, int player2ID, ApplicationDbContext dbContext)
        {
            return Task.FromResult<AnalyzerData>(null);
=======
        public override AnalyzerData Analyze(int player1ID, int player2ID, ApplicationDbContext dbContext)
        {
            throw new System.NotImplementedException();
>>>>>>> Implementing AnalyzerService
=======
        public override Task<AnalyzerData> Analyze(int player1ID, int player2ID, ApplicationDbContext dbContext)
        {
            return Task.FromResult<AnalyzerData>(null);
>>>>>>> Send data to WPF app
        }
    }
}