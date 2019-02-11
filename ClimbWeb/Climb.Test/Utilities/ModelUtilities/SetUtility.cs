using System;
using System.Collections.Generic;
using Climb.Data;
using Climb.Models;

namespace Climb.Test.Utilities
{
    public static class SetUtility
    {
        public static Set Create(this ApplicationDbContext dbContext, int player1ID, int player2ID, int leagueID, Season season = null)
        {
            return dbContext.AddAndSave(new Set(leagueID, player1ID, player2ID, DateTime.MaxValue, season?.ID));
        }

        public static Set Create(this ApplicationDbContext dbContext, SeasonLeagueUser player1, SeasonLeagueUser player2, int leagueID)
        {
            return dbContext.AddAndSave(new Set(leagueID, player1.LeagueUserID, player2.LeagueUserID, DateTime.MaxValue, player1.Season.ID, player1.ID, player2.ID));
        }

        public static Set CreateWithSeason(this ApplicationDbContext dbContext)
        {
            dbContext.CreateGame(3, 3);

            var (season, members) = dbContext.CreateSeason(2);
            var set = Create(dbContext, members[0].ID, members[1].ID, season.LeagueID, season);
            return set;
        }

        public static Set CreateWithLeague(this ApplicationDbContext dbContext)
        {
            dbContext.CreateGame(3, 3);

            var league = dbContext.CreateLeague(2);
            var set = Create(dbContext, league.Members[0].ID, league.Members[1].ID, league.ID);
            return set;
        }

        public static List<Match> AddMatches(this ApplicationDbContext dbContext, Set set, int count)
        {
            return dbContext.AddNewRange<Match>(count, (m, i) =>
            {
                m.Index = i;
                m.SetID = set.ID;
            });
        }
    }
}