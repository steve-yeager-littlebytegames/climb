<<<<<<< HEAD
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
=======
﻿using System.Threading.Tasks;
using Climb.Data;
>>>>>>> data-analyzer

namespace Climb.Services.DataAnalyzers
{
    public class SeasonRecordAnalyzer : DataAnalyzer
    {
<<<<<<< HEAD
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
=======
        public override Task<AnalyzerData> Analyze(int player1ID, int player2ID, ApplicationDbContext dbContext)
        {
            return Task.FromResult<AnalyzerData>(null);
>>>>>>> data-analyzer
        }
    }
}