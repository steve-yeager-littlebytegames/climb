using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Climb.Data;
using Climb.Models;
using Microsoft.EntityFrameworkCore;

namespace Climb.Services.DataAnalyzers
{
    public class SeasonRecordAnalyzer : DataAnalyzer
    {
        public override async Task<ICollection<AnalyzerData>> Analyze(int player1ID, int player2ID, ApplicationDbContext dbContext)
        {
            var data = new AnalyzerData("Last Season");

            var seasons = await dbContext.Seasons
                .Include(s => s.Participants).ThenInclude(slu => slu.LeagueUser)
                .OrderByDescending(s => s.Index)
                .ToArrayAsync();

            var hasP1 = false;
            var hasP2 = false;
            var index = 0;

            while(!hasP1 || !hasP2)
            {
                var season = seasons[index++];
                hasP1 = await GetPlayerData(hasP1, season, player1ID, dbContext, data.Player1Data);
                hasP2 = await GetPlayerData(hasP2, season, player2ID, dbContext, data.Player2Data);
            }

            return new[] {data};
        }

        private static async Task<bool> GetPlayerData(bool hasData, Season season, int playerID, ApplicationDbContext dbContext, List<string> data)
        {
            if(hasData)
            {
                return true;
            }

            var participant = season.Participants.FirstOrDefault(slu => slu.LeagueUserID == playerID);
            if(participant == null)
            {
                return false;
            }

            await dbContext.Entry(participant).Collection(slu => slu.P1Sets).LoadAsync();
            await dbContext.Entry(participant).Collection(slu => slu.P2Sets).LoadAsync();

            var sets = participant.P1Sets.Where(s => s.IsComplete)
                .Union(participant.P2Sets.Where(s => s.IsComplete))
                .ToArray();

            var wins = sets.Count(s => s.WinnerID == playerID);
            var losses = sets.Length - wins;

            data.Add($"Standing {participant.Standing}");
            data.Add($"{participant.Points} Points");
            data.Add($"Record of {wins}-{losses}");

            return true;
        }
    }
}