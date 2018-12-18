using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Climb.Data;
using Microsoft.EntityFrameworkCore;

namespace Climb.Services.DataAnalyzers
{
    public class MatchAnalyzer : DataAnalyzer
    {
        public override async Task<IReadOnlyList<string>> Analyze(int player1ID, int player2ID, ApplicationDbContext dbContext)
        {
            var data = new List<string>();
            await GetPlayerData(player1ID, dbContext, data);
            await GetPlayerData(player2ID, dbContext, data);

            return data;
        }

        private static async Task GetPlayerData(int playerID, ApplicationDbContext dbContext, List<string> data)
        {
            var player = await dbContext.LeagueUsers.FirstAsync(lu => lu.ID == playerID);

            var sets = await dbContext.Sets
                .IgnoreQueryFilters()
                .Include(s => s.Matches).ThenInclude(m => m.MatchCharacters).AsNoTracking()
                .Where(s => s.IsComplete && s.IsPlaying(playerID))
                .ToArrayAsync();

            var forfeitCount = 0;
            var clearWinnerCount = 0;
            var clearLoserCount = 0;

            foreach(var set in sets)
            {
                if(set.IsForfeit)
                {
                    ++forfeitCount;
                    continue;
                }

                if(set.Player1Score == 0 || set.Player1Score == 0)
                {
                    if(set.WinnerID == playerID)
                    {
                        ++clearWinnerCount;
                    }
                    else
                    {
                        ++clearLoserCount;
                    }
                }
            }

            // characters
            // stages

            var username = player.DisplayName;
            
            data.Add($"{username} has forfeited '{forfeitCount}' sets.");
            data.Add($"{username} has dominated '{clearWinnerCount}' times and has been dominated '{clearLoserCount}' times.");
        }
    }
}