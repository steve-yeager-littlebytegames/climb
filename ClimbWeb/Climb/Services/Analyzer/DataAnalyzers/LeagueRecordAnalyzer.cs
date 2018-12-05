<<<<<<< master
﻿using System.Threading.Tasks;
using Climb.Data;
using Microsoft.EntityFrameworkCore;
=======
﻿using Climb.Data;
>>>>>>> Implementing AnalyzerService

namespace Climb.Services.DataAnalyzers
{
    public class LeagueRecordAnalyzer : DataAnalyzer
    {
<<<<<<< master
        private class Data
        {
            public int Rank { get; set; }
            public double WeeksInLeague { get; set; }
        }

        private readonly IDateService dateService;

        public LeagueRecordAnalyzer(IDateService dateService)
        {
            this.dateService = dateService;
        }

        public override async Task<AnalyzerData> Analyze(int player1ID, int player2ID, ApplicationDbContext dbContext)
        {
            var data = new PlayerData<Data>("League Data")
            {
                Player1Data = await GetDataForPlayerAsync(player1ID, dbContext),
                Player2Data = await GetDataForPlayerAsync(player2ID, dbContext)
            };

            return data;
        }

        private async Task<Data> GetDataForPlayerAsync(int playerID, ApplicationDbContext dbContext)
        {
            var player = await dbContext.LeagueUsers.FirstAsync(lu => lu.ID == playerID);
            var data = new Data
            {
                Rank = player.Rank,
                WeeksInLeague = (dateService.Now - player.JoinDate).TotalDays / 7,
            };

            return data;
=======
        public override AnalyzerData Analyze(int player1ID, int player2ID, ApplicationDbContext dbContext)
        {
            throw new System.NotImplementedException();
>>>>>>> Implementing AnalyzerService
        }
    }
}