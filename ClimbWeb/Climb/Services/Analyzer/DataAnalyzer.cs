using System.Threading.Tasks;
using Climb.Data;

namespace Climb.Services
{
    public abstract class DataAnalyzer
    {
        public abstract Task<AnalyzerData> Analyze(int player1ID, int player2ID, ApplicationDbContext dbContext);
    }
}