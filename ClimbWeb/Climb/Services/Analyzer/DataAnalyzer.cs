<<<<<<< master
<<<<<<< master
﻿using System.Threading.Tasks;
using Climb.Data;
=======
﻿using Climb.Data;
>>>>>>> Setup class and start API tests
=======
﻿using System.Threading.Tasks;
using Climb.Data;
>>>>>>> Send data to WPF app

namespace Climb.Services
{
    public abstract class DataAnalyzer
    {
<<<<<<< master
<<<<<<< master
        public abstract Task<AnalyzerData> Analyze(int player1ID, int player2ID, ApplicationDbContext dbContext);
=======
        public abstract AnalyzerData Analyze(int player1ID, int player2ID, ApplicationDbContext dbContext);
>>>>>>> Setup class and start API tests
=======
        public abstract Task<AnalyzerData> Analyze(int player1ID, int player2ID, ApplicationDbContext dbContext);
>>>>>>> Send data to WPF app
    }
}