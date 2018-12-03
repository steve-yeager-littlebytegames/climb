using System.Threading.Tasks;
using Climb.Data;

namespace Climb.Services
{
    public class AnalyzerService : IAnalyzerService
    {
        private readonly ApplicationDbContext dbContext;

        public AnalyzerService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public Task<AnalyzerDataCollection> Calculate(int player1ID, int player2ID)
        {
            throw new System.NotImplementedException();
        }
    }
}