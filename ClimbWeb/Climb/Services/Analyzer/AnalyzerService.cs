using System.Threading.Tasks;
using Climb.Data;
using Climb.Exceptions;
using Climb.Models;
using Microsoft.EntityFrameworkCore;

namespace Climb.Services
{
    public class AnalyzerService : IAnalyzerService
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IDateService dateService;
        private readonly IAnalyzerFactory analyzerFactory;

        public AnalyzerService(ApplicationDbContext dbContext, IDateService dateService, IAnalyzerFactory analyzerFactory)
        {
            this.dbContext = dbContext;
            this.dateService = dateService;
            this.analyzerFactory = analyzerFactory;
        }

        public async Task<AnalyzerDataCollection> Calculate(int player1ID, int player2ID)
        {
            await VerifyPlayer(player1ID);
            await VerifyPlayer(player2ID);

            var analyzers = analyzerFactory.CreateAnalyzers();
            var dataCollection = new AnalyzerDataCollection(player1ID, player2ID, dateService.Now);

            foreach(var analyzer in analyzers)
            {
                var data = analyzer.Analyze(player1ID, player2ID, dbContext);
                if(data != null)
                {
                    dataCollection.Data.Add(data);
                }
            }

            return dataCollection;

            async Task VerifyPlayer(int id)
            {
                var player = await dbContext.LeagueUsers.FirstOrDefaultAsync(lu => lu.ID == id);
                if(player == null)
                {
                    throw new NotFoundException(typeof(LeagueUser), id);
                }
            }
        }
    }
}