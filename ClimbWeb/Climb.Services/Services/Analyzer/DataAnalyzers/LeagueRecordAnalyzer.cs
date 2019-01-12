using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Climb.Data;
using Microsoft.EntityFrameworkCore;

namespace Climb.Services.DataAnalyzers
{
    public class LeagueRecordAnalyzer : DataAnalyzer
    {
        private readonly IDateService dateService;

        public LeagueRecordAnalyzer(IDateService dateService)
        {
            this.dateService = dateService;
        }

        public override async Task<ICollection<AnalyzerData>> Analyze(int player1ID, int player2ID, ApplicationDbContext dbContext)
        {
            var data = new AnalyzerData("League");
            await GetPlayerDataAsync(player1ID, dbContext, data.Player1Data);
            await GetPlayerDataAsync(player2ID, dbContext, data.Player2Data);

            return new[] {data};
        }

        private async Task GetPlayerDataAsync(int playerID, ApplicationDbContext dbContext, ICollection<string> data)
        {
            var player = await dbContext.LeagueUsers.FirstAsync(lu => lu.ID == playerID);

            var weeksInLeague = Math.Floor((dateService.Now - player.JoinDate).TotalDays / 7);

            data.Add($"Rank '{player.Rank}'");
            data.Add($"Member for '{weeksInLeague}' weeks.");
        }
    }
}