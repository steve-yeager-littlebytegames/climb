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
        private readonly IBracketGenerator bracketGenerator;

        public TournamentService(ApplicationDbContext dbContext, IBracketGenerator bracketGenerator)
        {
            this.dbContext = dbContext;
            this.bracketGenerator = bracketGenerator;
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

            await CreateCompetitors();

            dbContext.Add(tournament);
            await dbContext.SaveChangesAsync();

            return tournament;

            async Task CreateCompetitors()
            {
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
            }
        }

        public async Task<Tournament> GenerateBracket(int tournamentID)
        {
            var tournament = await dbContext.Tournaments
                .Include(s => s.TournamentUsers).AsNoTracking()
                .Include(t => t.SetSlots)
                .FirstOrDefaultAsync(t => t.ID == tournamentID);
            dbContext.Update(tournament);

            var tournamentData = bracketGenerator.Generate(tournament.TournamentUsers.Count);

            dbContext.SetSlots.RemoveRange(tournament.SetSlots);
            tournament.SetSlots.Clear();

            tournament.SetSlots.AddRange(tournamentData.Winners.SelectMany(r => r.Games.Select(g => CreateSlot(g, SetSlot.Brackets.Winners))));
            tournament.SetSlots.AddRange(tournamentData.Losers.SelectMany(r => r.Games.Select(g => CreateSlot(g, SetSlot.Brackets.Losers))));
            tournament.SetSlots.AddRange(tournamentData.GrandFinals.Games.Select(g => CreateSlot(g, SetSlot.Brackets.Grands)));

            dbContext.AddRange(tournament.SetSlots);
            await dbContext.SaveChangesAsync();

            return tournament;

            SetSlot CreateSlot(BracketGenerator.GameData game, SetSlot.Brackets bracket)
            {
                return new SetSlot
                {
                    TournamentID = tournamentID,
                    Identifier = game.ID,
                    WinSlotIdentifier = game.NextWin.ID,
                    LoseSlotIdentifier = game.NextLoss.ID,
                    Bracket = bracket,
                };
            }
        }
    }
}