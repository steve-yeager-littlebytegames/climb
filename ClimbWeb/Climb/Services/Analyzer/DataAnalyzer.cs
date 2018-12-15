using System.Collections.Generic;
using System.Threading.Tasks;
using Climb.Data;

namespace Climb.Services
{
    public abstract class DataAnalyzer
    {
        public abstract Task<IReadOnlyList<string>> Analyze(int player1ID, int player2ID, ApplicationDbContext dbContext);
    }
}