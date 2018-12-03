<<<<<<< master
﻿using System.Threading.Tasks;
using Climb.Data;
=======
﻿using Climb.Data;
>>>>>>> Setup class and start API tests

namespace Climb.Services
{
    public abstract class DataAnalyzer
    {
<<<<<<< master
        public abstract Task<AnalyzerData> Analyze(int player1ID, int player2ID, ApplicationDbContext dbContext);
=======
        public abstract AnalyzerData Analyze(int player1ID, int player2ID, ApplicationDbContext dbContext);
>>>>>>> Setup class and start API tests
    }
}