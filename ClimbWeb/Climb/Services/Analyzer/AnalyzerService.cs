using System.Threading.Tasks;
using Climb.Data;
<<<<<<< master
using Climb.Exceptions;
using Climb.Models;
using Microsoft.EntityFrameworkCore;
=======
>>>>>>> Setup class and start API tests

namespace Climb.Services
{
    public class AnalyzerService : IAnalyzerService
    {
        private readonly ApplicationDbContext dbContext;
<<<<<<< master
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
                var data = await analyzer.Analyze(player1ID, player2ID, dbContext);
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
=======

        public AnalyzerService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public Task<AnalyzerDataCollection> Calculate(int player1ID, int player2ID)
        {
            throw new System.NotImplementedException();
>>>>>>> Setup class and start API tests
        }
    }
}