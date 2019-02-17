using System;
using System.Collections.Generic;
using Climb.Data;
using Climb.Models;

namespace Climb.Test.Utilities
{
    public static class TournamentUtility
    {
        public static Tournament CreateTournament(this ApplicationDbContext dbContext, DateTime createDate, League league = null)
        {
            league = league ?? dbContext.CreateLeague();
            var tournament = new Tournament(league.ID, createDate);
            dbContext.AddAndSave(tournament);
            return tournament;
        }

        public static List<TournamentUser> AddCompetitors(this ApplicationDbContext dbContext, Tournament tournament, int count)
        {
            var members = dbContext.AddUsersToLeague(tournament.League, count);
            var competitors = new List<TournamentUser>(count);
            for(var i = 0; i < count; i++)
            {
                var member = members[i];
                competitors.Add(dbContext.AddAndSave(new TournamentUser(tournament, member.UserID, member.ID, count - i)));
            }

            return competitors;
        }
    }
}