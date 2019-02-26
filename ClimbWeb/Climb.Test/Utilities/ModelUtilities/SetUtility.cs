using System.Collections.Generic;
using Climb.Data;
using Climb.Models;

namespace Climb.Test.Utilities
{
    public static class SetUtility
    {
        public static Set Create(ApplicationDbContext dbContext, int player1ID, int player2ID, int leagueID, Season season = null)
        {
            var set = DbContextUtility.AddNew<Set>(dbContext, s =>
            {
                s.Player1ID = player1ID;
                s.Player2ID = player2ID;
                s.LeagueID = leagueID;
                s.SeasonID = season?.ID;
            });

            return set;
        }

        public static Set Create(ApplicationDbContext dbContext, SeasonLeagueUser player1, SeasonLeagueUser player2, int leagueID)
        {
            var set = DbContextUtility.AddNew<Set>(dbContext, s =>
            {
                s.LeagueID = leagueID;
                s.SeasonID = player1.Season.ID;
                s.Player1ID = player1.LeagueUserID;
                s.Player2ID = player2.LeagueUserID;
                s.SeasonPlayer1 = player1;
                s.SeasonPlayer2 = player2;
                s.Player1 = player1.LeagueUser;
                s.Player2 = player2.LeagueUser;
            });

            return set;
        }

        public static Set Create(ApplicationDbContext dbContext)
        {
            GameUtility.CreateGame(dbContext, 3, 3);

            var (season, members) = SeasonUtility.CreateSeason(dbContext, 2);
            var set = Create(dbContext, members[0].ID, members[1].ID, season.LeagueID, season);
            return set;
        }

        public static List<Match> AddMatches(ApplicationDbContext dbContext, Set set, int count)
        {
            return DbContextUtility.AddNewRange<Match>(dbContext, count, (m, i) =>
            {
                m.Index = i;
                m.SetID = set.ID;
            });
        }
    }
}