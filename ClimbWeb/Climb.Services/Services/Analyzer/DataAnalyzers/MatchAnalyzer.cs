using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Climb.Data;
using Climb.Models;
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
                .Include(s => s.Matches).ThenInclude(m => m.MatchCharacters).ThenInclude(mc => mc.Character).AsNoTracking()
                .Include(s => s.Matches).ThenInclude(m => m.Stage).AsNoTracking()
                .Where(s => s.IsComplete && s.IsPlaying(playerID))
                .ToArrayAsync();

            var forfeitCount = 0;
            var clearWinnerCount = 0;
            var clearLoserCount = 0;

            var characters = new Dictionary<Character, (int wins, int losses)>();
            var stages = new Dictionary<Stage, (int wins, int losses)>();

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
                    var won = match.Player1Score > match.Player2Score && set.Player1ID == playerID;

                    foreach(var matchCharacter in match.MatchCharacters.Where(mc => mc.LeagueUserID != playerID))
                    {
                        if(!characters.TryGetValue(matchCharacter.Character, out var character))
                        {
                            character = (0, 0);
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
                            stage = (0, 0);
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

            var username = player.DisplayName;

            var (bestStage, (bestStageWins, bestStageLosses)) = stages.OrderByDescending(s => s.Value.wins).FirstOrDefault();
            var (worstStage, (worstStageWins, worstStageLosses)) = stages.OrderByDescending(s => s.Value.losses).FirstOrDefault();

            var (bestCharacter, (bestCharWins, bestCharLosses)) = characters.OrderByDescending(c => c.Value.wins).FirstOrDefault();
            var (worstCharacter, (worstCharWins, worstCharLosses)) = characters.OrderByDescending(c => c.Value.losses).FirstOrDefault();

            data.Add($"{username} has forfeited '{forfeitCount}' sets.");
            data.Add($"{username} has dominated '{clearWinnerCount}' times and has been dominated '{clearLoserCount}' times.");
            data.Add($"{username}'s best stage is '{bestStage.Name}' with a record of '{bestStageWins}-{bestStageLosses}'.");
            data.Add($"{username}'s worst stage is '{worstStage.Name}' with a record of '{worstStageWins}-{worstStageLosses}'.");
            data.Add($"{username}'s best against '{bestCharacter.Name}' with a record of '{bestCharWins}-{bestCharLosses}'.");
            data.Add($"{username}'s worst against '{worstCharacter.Name}' with a record of '{worstCharWins}-{worstCharLosses}'.");
        }
    }
}