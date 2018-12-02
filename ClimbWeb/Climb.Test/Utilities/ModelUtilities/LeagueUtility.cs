using System.Collections.Generic;
using Climb.Data;
using Climb.Models;

namespace Climb.Test.Utilities
{
    public static class LeagueUtility
    {
        public static League CreateLeague(this ApplicationDbContext dbContext, int memberCount = 0, string adminID = null)
        {
            var game = dbContext.AddNew<Game>();
            var league = dbContext.AddNew<League>(l =>
            {
                l.GameID = game.ID;
                if(!string.IsNullOrWhiteSpace(adminID))
                {
                    l.AdminID = adminID;
                }
            });

            if(memberCount > 0)
            {
                AddUsersToLeague(dbContext, league, memberCount);
            }

            return league;
        }

        public static List<LeagueUser> AddUsersToLeague(this ApplicationDbContext dbContext, League league, int count)
        {
            var users = dbContext.AddNewRange<ApplicationUser>(count);
            return dbContext.AddNewRange<LeagueUser>(count, (lu, i) =>
            {
                lu.UserID = users[i].Id;
                lu.LeagueID = league.ID;
            });
        }
    }
}