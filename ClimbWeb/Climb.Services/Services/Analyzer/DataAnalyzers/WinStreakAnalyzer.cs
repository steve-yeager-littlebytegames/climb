using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Climb.Data;
using Climb.Models;
using Microsoft.EntityFrameworkCore;

namespace Climb.Services.DataAnalyzers
{
    public class WinStreakAnalyzer : DataAnalyzer
    {
        public override async Task<IReadOnlyList<string>> Analyze(int player1ID, int player2ID, ApplicationDbContext dbContext)
        {
            var data = new List<string>();
            await GetPlayerData(player1ID, data, dbContext);
            await GetPlayerData(player2ID, data, dbContext);

            return data;
        }

        private static async Task GetPlayerData(int playerID, List<string> data, ApplicationDbContext dbContext)
        {
            var player = await dbContext.LeagueUsers.FirstAsync(lu => lu.ID == playerID);

            var sets = await dbContext.Sets
                .Include(s => s.Player1).AsNoTracking()
                .Include(s => s.Player2).AsNoTracking()
                .Where(s => s.IsComplete)
                .Where(s => s.Player1ID == playerID || s.Player2ID == playerID)
                .OrderBy(s => s.UpdatedDate)
                .ToArrayAsync();

            var bestStart = DateTime.MinValue;
            var bestEnd = DateTime.MinValue;
            var bestCount = 0;

            var currentStart = DateTime.MinValue;
            var currentEnd = DateTime.MinValue;
            var currentCount = 0;

            var isStreak = false;
            LeagueUser streakBreaker = null;

            foreach(var set in sets)
            {
                if(set.WinnerID == playerID)
                {
                    if(isStreak)
                    {
                        ++currentCount;
                        currentEnd = set.UpdatedDate ?? set.DueDate;
                    }
                    else
                    {
                        isStreak = true;
                        currentCount = 1;
                        currentStart = set.UpdatedDate ?? set.DueDate;
                    }
                }
                else
                {
                    isStreak = false;
                    streakBreaker = set.WinnerID == set.Player1ID ? set.Player1 : set.Player2;

                    if(currentCount >= bestCount)
                    {
                        bestCount = currentCount;
                        bestStart = currentStart;
                        bestEnd = currentEnd;
                    }
                }
            }

            var isOngoing = false;
            if(currentCount >= bestCount)
            {
                isOngoing = true;
                bestCount = currentCount;
                bestStart = currentStart;
                bestEnd = currentEnd;
            }

            var streakWeeks = Math.Floor((bestEnd - bestStart).TotalDays / 7);

            var result = $"{player.DisplayName}'s best Win Streak is '{bestCount}' " +
                         $"sets for '{streakWeeks}' weeks " +
                         $"from '{bestStart.ToShortDateString()}-{bestEnd.ToShortDateString()}'.";

            if(isOngoing)
            {
                result += " This is currently ongoing.";
            }
            else
            {
                Debug.Assert(streakBreaker != null, nameof(streakBreaker) + " != null");
                result += $" This was broken by {streakBreaker.DisplayName}";
            }

            data.Add(result);
        }
    }
}