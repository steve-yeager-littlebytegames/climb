using System.Collections.Generic;
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
        private readonly IAnalyzerFactory analyzerFactory;

        public AnalyzerService(ApplicationDbContext dbContext, IAnalyzerFactory analyzerFactory)
        {
            this.dbContext = dbContext;
            this.analyzerFactory = analyzerFactory;
        }

        public async Task<IReadOnlyList<string>> Calculate(int player1ID, int player2ID)
        {
            var player1 = await dbContext.LeagueUsers.FirstOrDefaultAsync(lu => lu.ID == player1ID);
            if(player1 == null)
            {
                throw new NotFoundException(typeof(LeagueUser), player1ID);
            }

            var player2 = await dbContext.LeagueUsers.FirstOrDefaultAsync(lu => lu.ID == player2ID);
            if(player2 == null)
            {
                throw new NotFoundException(typeof(LeagueUser), player2ID);
            }

            await VerifyPlayer(player1ID);
            await VerifyPlayer(player2ID);

            var analyzers = analyzerFactory.CreateAnalyzers();
            var dataCollection = new List<string>();

            foreach(var analyzer in analyzers)
            {
                var data = await analyzer.Analyze(player1ID, player2ID, dbContext);
                if(data != null)
                {
                    dataCollection.AddRange(data);
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