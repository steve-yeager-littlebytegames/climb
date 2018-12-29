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

        public override async Task<IReadOnlyList<string>> Analyze(int player1ID, int player2ID, ApplicationDbContext dbContext)
        {
            var data = new List<string>();
            await GetPlayerDataAsync(player1ID, dbContext, data);
            await GetPlayerDataAsync(player2ID, dbContext, data);

            return data;
        }

        private async Task GetPlayerDataAsync(int playerID, ApplicationDbContext dbContext, ICollection<string> data)
        {
            var player = await dbContext.LeagueUsers.FirstAsync(lu => lu.ID == playerID);

            var weeksInLeague = Math.Floor((dateService.Now - player.JoinDate).TotalDays / 7);

            data.Add($"{player.DisplayName} has Rank '{player.Rank}' and been in League for '{weeksInLeague}' weeks.");
        }
    }
}