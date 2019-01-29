using System;
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

        public static TournamentUser[] AddCompetitors(this ApplicationDbContext dbContext, Tournament tournament, int count)
        {
            var members = dbContext.AddUsersToLeague(tournament.League, count);
            var competitors = new TournamentUser[count];
            for(var i = 0; i < members.Count; i++)
            {
                var member = members[i];
                competitors[i] = dbContext.AddAndSave(new TournamentUser(tournament, member.UserID, member.ID, -1));
            }

            return competitors;
        }
    }
}