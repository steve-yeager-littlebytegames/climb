using System.Linq;
using System.Threading.Tasks;
using Climb.Data;
using Climb.Exceptions;
using Climb.Models;
using Microsoft.EntityFrameworkCore;

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
            if(!await dbContext.Leagues.AnyAsync(l => l.ID == leagueID))
            {
                throw new NotFoundException(typeof(League), leagueID);
            }

            var tournament = new Tournament
            {
                LeagueID = leagueID,
                SeasonID = seasonID,
                Name = name,
            };

            if(seasonID != null)
            {
                var season = await dbContext.Seasons
                    .Include(s => s.Participants)
                    .FirstOrDefaultAsync(s => s.ID == seasonID.Value);
                if(season == null)
                {
                    throw new NotFoundException(typeof(Season), seasonID.Value);
                }

                tournament.TournamentUsers = season.Participants
                    .Select(slu => new TournamentUser(slu) {Seed = slu.Standing})
                    .ToList();
                dbContext.TournamentUsers.AddRange(tournament.TournamentUsers);
            }

            dbContext.Add(tournament);
            await dbContext.SaveChangesAsync();

            return tournament;
        }
    }
}