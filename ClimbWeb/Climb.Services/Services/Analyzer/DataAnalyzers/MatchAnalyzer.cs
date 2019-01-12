using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Climb.Data;
using Climb.Models;
using Microsoft.EntityFrameworkCore;

namespace Climb.Services.DataAnalyzers
{
    // collect data in a new class for each player
    // iterate data and create analyzerdatas
    public class MatchAnalyzer : DataAnalyzer
    {
        private class Record
        {
            public int wins;
            public int losses;

            public decimal Ratio => (wins - losses) * (wins + losses);
        }

        public override async Task<ICollection<AnalyzerData>> Analyze(int player1ID, int player2ID, ApplicationDbContext dbContext)
        {
            var p1Data = await GetPlayerData(player1ID, dbContext);
            var p2Data = await GetPlayerData(player2ID, dbContext);

            var data = new List<AnalyzerData>(3);
            for(int i = 0; i < p1Data.Count; i++)
            {
                var analyzerData = new A
            }

            return data;
        }

        private static async Task<List<List<string>>> GetPlayerData(int playerID, ApplicationDbContext dbContext)
        {
            var sets = await dbContext.Sets
                .IgnoreQueryFilters()
                .Include(s => s.Matches).ThenInclude(m => m.MatchCharacters).ThenInclude(mc => mc.Character).AsNoTracking()
                .Include(s => s.Matches).ThenInclude(m => m.Stage).AsNoTracking()
                .Where(s => s.IsComplete && s.IsPlaying(playerID))
                .ToArrayAsync();

            var forfeitCount = 0;
            var clearWinnerCount = 0;
            var clearLoserCount = 0;

            var characters = new Dictionary<Character, Record>();
            var stages = new Dictionary<Stage, Record>();

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

                foreach(var match in set.Matches)
                {
                    var won = match.Player1Score > match.Player2Score && set.Player1ID == playerID
                              || match.Player2Score > match.Player1Score && set.Player2ID == playerID;

                    foreach(var matchCharacter in match.MatchCharacters.Where(mc => mc.LeagueUserID != playerID))
                    {
                        if(!characters.TryGetValue(matchCharacter.Character, out var character))
                        {
                            character = new Record();
                            characters[matchCharacter.Character] = character;
                        }

                        if(won)
                        {
                            ++character.wins;
                        }
                        else
                        {
                            ++character.losses;
                        }
                    }

                    if(match.Stage != null)
                    {
                        if(!stages.TryGetValue(match.Stage, out var stage))
                        {
                            stage = new Record();
                            stages[match.Stage] = stage;
                        }

                        if(won)
                        {
                            ++stage.wins;
                        }
                        else
                        {
                            ++stage.losses;
                        }
                    }
                }
            }

            const int recordLimit = 0;

            var sortedStages = stages.OrderByDescending(s => s.Value.Ratio).ToArray();
            var (_, bestStageRecord) = sortedStages.FirstOrDefault(s => s.Value.wins > recordLimit);
            var (_, worstStageRecord) = sortedStages.LastOrDefault(s => s.Value.losses > recordLimit);

            var bestStages = sortedStages.Where(s => s.Value.Ratio == bestStageRecord.Ratio).ToArray();
            var worstStages = sortedStages.Where(s => s.Value.Ratio == worstStageRecord.Ratio).ToArray();

            var sortedCharacters = characters.OrderByDescending(c => c.Value.Ratio).ToArray();
            var (_, bestCharRecord) = sortedCharacters.FirstOrDefault(c => c.Value.wins > recordLimit);
            var (_, worstCharRecord) = sortedCharacters.LastOrDefault(c => c.Value.losses > recordLimit);

            var bestCharacters = sortedCharacters.Where(c => c.Value.Ratio == bestCharRecord.Ratio).ToArray();
            var worstCharacters = sortedCharacters.Where(c => c.Value.Ratio == worstCharRecord.Ratio).ToArray();

            data.Add($"Forfeited '{forfeitCount}' sets.");
            data.Add($"Dominated '{clearWinnerCount}' times.");
            data.Add($"Been dominated '{clearLoserCount}' times.");
            data.Add($"Best on '{string.Join(", ", bestStages.Select(s => s.Key.Name))}' with a record of '{bestStageRecord.wins}-{bestStageRecord.losses}'.");
            data.Add($"Worst on '{string.Join(", ", worstStages.Select(s => s.Key.Name))}' with a record of '{worstStageRecord.wins}-{worstStageRecord.losses}'.");
            data.Add($"Best against '{string.Join(", ", bestCharacters.Select(s => s.Key.Name))}' with a record of '{bestCharRecord.wins}-{bestCharRecord.losses}'.");
            data.Add($"Worst against '{string.Join(", ", worstCharacters.Select(s => s.Key.Name))}' with a record of '{worstCharRecord.wins}-{worstCharRecord.losses}'.");
        }
    }
}