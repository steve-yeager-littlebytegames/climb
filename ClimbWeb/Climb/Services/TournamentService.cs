using System.Threading.Tasks;
using Climb.Data;
using Climb.Models;

namespace Climb.Services
{
    public class TournamentService : ITournamentService
    {
        private readonly ApplicationDbContext dbContext;

        public TournamentService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<Tournament> Create(int leagueID, int? seasonID, string name)
        {
            var tournament = new Tournament
            {
                LeagueID = leagueID,
                SeasonID = seasonID,
                Name = name,
            };

            dbContext.Add(tournament);
            await dbContext.SaveChangesAsync();

            return tournament;
        }
    }
}