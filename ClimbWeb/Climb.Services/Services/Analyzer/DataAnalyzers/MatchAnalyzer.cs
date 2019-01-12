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
        private class Record
        {
            public int wins;
            public int losses;

            public decimal Ratio => (wins - losses) * (wins + losses);
        }

        private class PlayerData
        {
            public int Forfeited { get; }
            public int ClearWins { get; }
            public int ClearLosses { get; }
            public Record BestStageRecord { get; private set; }
            public ICollection<string> BestStages { get; private set; }
            public Record WorstStageRecord { get; private set; }
            public ICollection<string> WorstStages { get; private set; }
            public Record BestCharRecord { get; private set; }
            public ICollection<string> BestChars { get; private set; }
            public Record WorstCharRecord { get; private set; }
            public ICollection<string> WorstChars { get; private set; }

            public PlayerData(int forfeited, int clearWins, int clearLosses)
            {
                Forfeited = forfeited;
                ClearWins = clearWins;
                ClearLosses = clearLosses;
            }

            public void SetStages(Record bestRecord, Record worstRecord, KeyValuePair<Stage, Record>[] stages)
            {
                BestStageRecord = bestRecord;
                WorstStageRecord = worstRecord;

                BestStages = stages.Where(s => s.Value.Ratio == bestRecord.Ratio).Select(s => s.Key.Name).ToArray();
                WorstStages = stages.Where(s => s.Value.Ratio == worstRecord.Ratio).Select(s => s.Key.Name).ToArray();
            }

            public void SetCharacters(Record bestRecord, Record worstRecord, KeyValuePair<Character, Record>[] characters)
            {
                BestCharRecord = bestRecord;
                WorstCharRecord = worstRecord;

                BestChars = characters.Where(s => s.Value.Ratio == bestRecord.Ratio).Select(s => s.Key.Name).ToArray();
                WorstChars = characters.Where(s => s.Value.Ratio == worstRecord.Ratio).Select(s => s.Key.Name).ToArray();
            }
        }

        public override async Task<ICollection<AnalyzerData>> Analyze(int player1ID, int player2ID, ApplicationDbContext dbContext)
        {
            var p1Data = await GetPlayerData(player1ID, dbContext);
            var p2Data = await GetPlayerData(player2ID, dbContext);

            var data = new List<AnalyzerData>(3);   
            SetsForfeited(data, p1Data, p2Data);
            ClearSets(data, p1Data, p2Data);
            Stages(data, p1Data, p2Data);
            Characters(data, p1Data, p2Data);

            return data;
        }

        private static void SetsForfeited(ICollection<AnalyzerData> data, PlayerData p1, PlayerData p2)
        {
            var analyzerData = new AnalyzerData("Sets Forfeited");
            data.Add(analyzerData);
            analyzerData.Player1Data.Add(p1.Forfeited.ToString());
            analyzerData.Player2Data.Add(p2.Forfeited.ToString());
        }

        private static void ClearSets(ICollection<AnalyzerData> data, PlayerData p1, PlayerData p2)
        {
            var analyzerData = new AnalyzerData("Clear Victories");
            data.Add(analyzerData);
            analyzerData.Player1Data.Add($"Won {p1.ClearWins}");
            analyzerData.Player1Data.Add($"Lost {p1.ClearLosses}");
            analyzerData.Player2Data.Add($"Won {p2.ClearWins}");
            analyzerData.Player2Data.Add($"Lost {p2.ClearLosses}");
        }

        private static void Stages(ICollection<AnalyzerData> data, PlayerData p1Data, PlayerData p2Data)
        {
            var analyzerData = new AnalyzerData("Stages");
            data.Add(analyzerData);
            analyzerData.Player1Data.Add($"Best on '{string.Join(", ", p1Data.BestStages)}' with a record of '{p1Data.BestStageRecord.wins}-{p1Data.BestStageRecord.losses}'.");
            analyzerData.Player1Data.Add($"Worst on '{string.Join(", ", p1Data.WorstStages)}' with a record of '{p1Data.WorstStageRecord.wins}-{p1Data.WorstStageRecord.losses}'.");
            analyzerData.Player2Data.Add($"Best on '{string.Join(", ", p2Data.BestStages)}' with a record of '{p2Data.BestStageRecord.wins}-{p2Data.BestStageRecord.losses}'.");
            analyzerData.Player2Data.Add($"Worst on '{string.Join(", ", p2Data.WorstStages)}' with a record of '{p2Data.WorstStageRecord.wins}-{p2Data.WorstStageRecord.losses}'.");
        }

        private static void Characters(ICollection<AnalyzerData> data, PlayerData p1Data, PlayerData p2Data)
        {
            var analyzerData = new AnalyzerData("Characters");
            data.Add(analyzerData);
            analyzerData.Player1Data.Add($"Best against '{string.Join(", ", p1Data.BestChars)}' with a record of '{p1Data.BestCharRecord.wins}-{p1Data.BestCharRecord.losses}'.");
            analyzerData.Player1Data.Add($"Worst against '{string.Join(", ", p1Data.WorstChars)}' with a record of '{p1Data.WorstCharRecord.wins}-{p1Data.WorstCharRecord.losses}'.");
            analyzerData.Player2Data.Add($"Best against '{string.Join(", ", p2Data.BestChars)}' with a record of '{p2Data.BestCharRecord.wins}-{p2Data.BestCharRecord.losses}'.");
            analyzerData.Player2Data.Add($"Worst against '{string.Join(", ", p2Data.WorstChars)}' with a record of '{p2Data.WorstCharRecord.wins}-{p2Data.WorstCharRecord.losses}'.");
        }

        private static async Task<PlayerData> GetPlayerData(int playerID, ApplicationDbContext dbContext)
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

            var sortedCharacters = characters.OrderByDescending(c => c.Value.Ratio).ToArray();
            var (_, bestCharRecord) = sortedCharacters.FirstOrDefault(c => c.Value.wins > recordLimit);
            var (_, worstCharRecord) = sortedCharacters.LastOrDefault(c => c.Value.losses > recordLimit);

            var data = new PlayerData(forfeitCount, clearWinnerCount, clearLoserCount);
            data.SetStages(bestStageRecord, worstStageRecord, sortedStages);
            data.SetCharacters(bestCharRecord, worstCharRecord, sortedCharacters);

            return data;
        }
    }
}