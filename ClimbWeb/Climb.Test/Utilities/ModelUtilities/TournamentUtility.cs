using Climb.Data;
using Climb.Models;

namespace Climb.Test.Utilities
{
    public static class TournamentUtility
    {
        public static Tournament CreateTournament(this ApplicationDbContext dbContext)
        {
            var league = dbContext.CreateLeague();
            var tournament = new Tournament(league.ID);
            dbContext.AddAndSave(tournament);
            return tournament;
        }
    }
}