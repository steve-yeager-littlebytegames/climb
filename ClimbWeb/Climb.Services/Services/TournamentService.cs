using System.Collections.Generic;
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
                        .Include(s => s.Participants).AsNoTracking()
                        .FirstOrDefaultAsync(s => s.ID == seasonID.Value);
                    if(season == null)
                    {
                        throw new NotFoundException(typeof(Season), seasonID.Value);
                    }

                    if(season.LeagueID != leagueID)
                    {
                        throw new BadRequestException(nameof(seasonID), $"The Season does not belong to the League ({season.LeagueID} vs {leagueID}).");
                    }

                    tournament.TournamentUsers = season.Participants
                        .Select(slu => new TournamentUser(tournament, slu.UserID, slu.LeagueUserID, slu.Standing, slu.ID))
                        .ToList();
                }
            }
        }

        public async Task<Tournament> GenerateBracket(int tournamentID)
        {
            var tournament = await dbContext.Tournaments
                .Include(s => s.TournamentUsers).AsNoTracking()
                .Include(t => t.Rounds)
                .Include(t => t.SetSlots)
                .FirstOrDefaultAsync(t => t.ID == tournamentID);
            dbContext.Update(tournament);

            dbContext.Rounds.RemoveRange(tournament.Rounds);
            tournament.Rounds.Clear();
            dbContext.SetSlots.RemoveRange(tournament.SetSlots);
            tournament.SetSlots.Clear();

            var tournamentData = bracketGenerator.Generate(tournament.TournamentUsers.Count);
            AddBracket(tournament, tournamentData);

            await dbContext.SaveChangesAsync();

            return tournament;
        }

        public void AddBracket(Tournament tournament, BracketGenerator.BracketData bracketData)
        {
            tournament.Rounds.AddRange(bracketData.Winners.Select(rd => CreateRound(rd, Round.Brackets.Winners)));
            tournament.Rounds.AddRange(bracketData.Losers.Select(rd => CreateRound(rd, Round.Brackets.Losers)));
            tournament.Rounds.AddRange(bracketData.GrandFinals.Select(rd => CreateRound(rd, Round.Brackets.Grands)));

            Round CreateRound(BracketGenerator.RoundData roundData, Round.Brackets bracket)
            {
                var round = new Round
                {
                    Index = roundData.Index,
                    TournamentID = tournament.ID,
                    Name = roundData.Name,
                    Bracket = bracket,
                };
                round.SetSlots = roundData.Games.Select(g => CreateSlot(g, round)).ToList();

                return round;
            }
        
            SetSlot CreateSlot(BracketGenerator.GameData game, Round round)
            {
                return new SetSlot
                {
                    TournamentID = tournament.ID,
                    Identifier = game.ID,
                    Round = round,
                    WinSlotIdentifier = game.NextWin?.ID,
                    LoseSlotIdentifier = game.NextLoss?.ID,
                    P1Game = game.P1Game?.ID,
                    P2Game = game.P2Game?.ID,
                    IsBye = game.IsBye,
                };
            }
        }

        public async Task<TournamentUser> Join(int tournamentID, string userID)
        {
            var tournament = await dbContext.Tournaments
                .Include(t => t.TournamentUsers)
                .FirstOrDefaultAsync(t => t.ID == tournamentID);
            if(tournament == null)
            {
                throw new NotFoundException(typeof(Tournament), tournamentID);
            }

            if(tournament.State == Tournament.States.Active)
            {
                throw new BadRequestException("Can't add user after tournament has been started.");
            }

            if(tournament.State == Tournament.States.Complete)
            {
                throw new BadRequestException("Can't add user after tournament has been completed.");
            }

            var member = await dbContext.LeagueUsers
                .FirstOrDefaultAsync(lu => lu.LeagueID == tournament.LeagueID && lu.UserID == userID);
            if(member == null)
            {
                throw new NotFoundException(typeof(ApplicationUser), userID);
            }

            var competitor = tournament.TournamentUsers.FirstOrDefault(tu => tu.UserID == userID);
            if(competitor == null)
            {
                competitor = new TournamentUser(tournament, member.UserID, member.ID, tournament.TournamentUsers.Count + 1);

                var seasonUser = await dbContext.SeasonLeagueUsers
                    .FirstOrDefaultAsync(su => su.LeagueUserID == member.ID && su.UserID == userID);
                if(seasonUser != null)
                {
                    competitor.SeasonLeagueUserID = seasonUser.ID;
                }

                await dbContext.AddAndSaveAsync(competitor);
            }

            return competitor;
        }

        public async Task Leave(int competitorID)
        {
            var competitor = await dbContext.TournamentUsers
                .Include(tu => tu.Tournament).ThenInclude(t => t.TournamentUsers)
                .FirstOrDefaultAsync(tu => tu.ID == competitorID);
            if(competitor == null)
            {
                throw new NotFoundException(typeof(TournamentUser), competitorID);
            }

            var tournament = competitor.Tournament;

            if(tournament.State == Tournament.States.Active)
            {
                throw new BadRequestException($"Tournament {tournament.ID} has already started.");
            }

            if(tournament.State == Tournament.States.Complete)
            {
                throw new BadRequestException($"Tournament {tournament.ID} has already finished.");
            }

            tournament.TournamentUsers.Remove(competitor);
            dbContext.Remove(competitor);

            dbContext.Update(tournament);
            SortSeeds(tournament.TournamentUsers);

            await dbContext.SaveChangesAsync();
        }

        private static void SortSeeds(List<TournamentUser> competitors)
        {
            competitors.Sort((x, y) => x.Seed.CompareTo(y.Seed));

            for(int i = 0; i < competitors.Count; i++)
            {
                competitors[i].Seed = i + 1;
            }
        }

        public async Task<Tournament> Start(int tournamentID)
        {
            var tournament = await dbContext.Tournaments
                .Include(t => t.Rounds).ThenInclude(r => r.SetSlots)
                .FirstOrDefaultAsync(t => t.ID == tournamentID);
            if(tournament == null)
            {
                throw new NotFoundException(typeof(Tournament), tournamentID);
            }

            dbContext.Update(tournament);

            var firstRound = tournament.GetRound(Round.Brackets.Winners, 1);
            foreach(var slot in firstRound.SetSlots)
            {
                var hasPlayers = true;
                if(hasPlayers)
                {
                    await CreateSetForSlot(slot);
                }
            }

            await dbContext.SaveChangesAsync();

            async Task CreateSetForSlot(SetSlot slot)
            {

            }

            return tournament;
        }
    }
}