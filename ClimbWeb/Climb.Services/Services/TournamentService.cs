using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private readonly IDateService dateService;

        public TournamentService(ApplicationDbContext dbContext, IBracketGenerator bracketGenerator, IDateService dateService)
        {
            this.dbContext = dbContext;
            this.bracketGenerator = bracketGenerator;
            this.dateService = dateService;
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
                CreateDate = dateService.Now,
            };

            await CreateCompetitors();

            UpdateBracket(tournament);

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

                UpdateBracket(tournament);

                await dbContext.AddAndSaveAsync(competitor);
            }

            return competitor;
        }

        public async Task Leave(int competitorID)
        {
            var competitor = await dbContext.TournamentUsers
                .Include(tu => tu.Tournament).ThenInclude(t => t.TournamentUsers)
                .Include(tu => tu.Tournament).ThenInclude(t => t.Rounds)
                .Include(tu => tu.Tournament).ThenInclude(t => t.SetSlots)
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
            UpdateBracket(tournament);

            await dbContext.SaveChangesAsync();
        }

        public async Task<Tournament> Start(int tournamentID)
        {
            var tournament = await dbContext.Tournaments
                .Include(t => t.SetSlots)
                .Include(t => t.Rounds).ThenInclude(r => r.SetSlots).ThenInclude(ss => ss.Set).ThenInclude(s => s.TournamentPlayer1)
                .Include(t => t.Rounds).ThenInclude(r => r.SetSlots).ThenInclude(ss => ss.Set).ThenInclude(s => s.TournamentPlayer2)
                .Include(t => t.TournamentUsers).AsNoTracking()
                .FirstOrDefaultAsync(t => t.ID == tournamentID);
            if(tournament == null)
            {
                throw new NotFoundException(typeof(Tournament), tournamentID);
            }

            dbContext.Update(tournament);

            // TODO: Clear old sets?
            tournament.Sets = new List<Set>();

            foreach(var slot in tournament.SetSlots)
            {
                if(slot.IsFull)
                {
                    // TODO: Dupe?
                    var set = CreateSet(tournament, slot, slot.User1, slot.User2, dateService.Now.AddDays(7));
                    slot.Set = set;
                    tournament.Sets.Add(set);
                }
            }

            await dbContext.SaveChangesAsync();

            return tournament;
        }

        public async Task FightSet(Set set)
        {
            var tournament = await dbContext.Tournaments
                .Include(t => t.SetSlots)
                .Include(t => t.Rounds).ThenInclude(r => r.SetSlots).ThenInclude(ss => ss.Set).ThenInclude(s => s.TournamentPlayer1)
                .Include(t => t.Rounds).ThenInclude(r => r.SetSlots).ThenInclude(ss => ss.Set).ThenInclude(s => s.TournamentPlayer2)
                .Include(t => t.TournamentUsers)
                .FirstAsync(t => t.ID == set.TournamentID);

            dbContext.Update(tournament);

            var slot = await dbContext.SetSlots
                .FindAsync(set.SetSlotID);

            var winner = set.Player1Score > set.Player2Score ? set.TournamentUser1ID : set.TournamentUser2ID;
            var loser = set.Player1Score > set.Player2Score ? set.TournamentUser2ID : set.TournamentUser1ID;
            Debug.Assert(winner != null, nameof(winner) + " != null");
            Debug.Assert(loser != null, nameof(loser) + " != null");

            await UpdateSlot(winner.Value, slot.WinSlotIdentifier);
            await UpdateSlot(loser.Value, slot.LoseSlotIdentifier);

            async Task UpdateSlot(int user, int? nextSlotIdentifier)
            {
                if(nextSlotIdentifier != null)
                {
                    var nextSlot = tournament.SetSlots.First(ss => ss.Identifier == nextSlotIdentifier);
                    nextSlot.AssignPlayer(slot, user);

                    if(nextSlot.IsFull)
                    {
                        // TODO: Don't do this.
                        var user1 = await dbContext.TournamentUsers.FindAsync(nextSlot.User1ID);
                        var user2 = await dbContext.TournamentUsers.FindAsync(nextSlot.User2ID);

                        var nextSet = CreateSet(tournament, nextSlot, user1, user2, dateService.Now.AddDays(7));
                        dbContext.Add(nextSet);
                        nextSlot.Set = nextSet;
                        tournament.Sets.Add(nextSet);
                    }
                }
                else
                {
                    // TODO: Give user place.
                }
            }
        }

        // TODO: should not be named this. this is for creating a bracket
        private void UpdateBracket(Tournament tournament)
        {
            if(tournament.TournamentUsers?.Count >= 4)
            {
                CreateBracket(tournament);
                PopulateBracket(tournament);
            }
        }

        private void CreateBracket(Tournament tournament)
        {
            Clear();
            var bracketData = bracketGenerator.Generate(tournament.TournamentUsers.Count);
            AddBracket();

            void Clear()
            {
                if(tournament.Rounds?.Count > 0)
                {
                    dbContext.Rounds.RemoveRange(tournament.Rounds);
                }

                tournament.Rounds = new List<Round>();

                if(tournament.SetSlots?.Count > 0)
                {
                    dbContext.SetSlots.RemoveRange(tournament.SetSlots);
                    tournament.SetSlots.Clear();
                }

                tournament.SetSlots = new List<SetSlot>();
            }

            void AddBracket()
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
                    tournament.SetSlots.AddRange(round.SetSlots);

                    return round;
                }

                SetSlot CreateSlot(BracketGenerator.GameData game, Round round)
                {
                    return new SetSlot
                    {
                        Tournament = tournament,
                        Identifier = game.ID,
                        Round = round,
                        WinSlotIdentifier = game.NextWin?.ID,
                        LoseSlotIdentifier = game.NextLoss?.ID,
                        P1Game = game.P1Game?.ID,
                        P2Game = game.P2Game?.ID,
                    };
                }
            }
        }

        private void PopulateBracket(Tournament tournament)
        {
            var users = PadUsers();
            SortUsers();
            ClearSlots();
            AssignToSlots();
            MarkLoserFirstRoundByes();

            List<TournamentUser> PadUsers()
            {
                var tournamentUsers = new List<TournamentUser>(tournament.TournamentUsers);
                var fullBracketCount = BracketGenerator.GetFullBracketCount(tournamentUsers.Count);

                tournamentUsers.Sort((a, b) => a.Seed.CompareTo(b.Seed));

                while(tournamentUsers.Count < fullBracketCount)
                {
                    tournamentUsers.Add(TournamentUser.NullUser);
                }

                return tournamentUsers;
            }

            // https://stackoverflow.com/a/6783584
            void SortUsers()
            {
                var slice = 1;
                while(slice < users.Count / 2)
                {
                    var tempUsers = new List<TournamentUser>(users);
                    users.Clear();

                    while(tempUsers.Count > 0)
                    {
                        users.AddRange(tempUsers.GetRange(0, slice));
                        tempUsers.RemoveRange(0, slice);
                        users.AddRange(tempUsers.GetRange(tempUsers.Count - slice, slice));
                        tempUsers.RemoveRange(tempUsers.Count - slice, slice);
                    }

                    slice *= 2;
                }
            }

            void ClearSlots()
            {
                foreach(var slot in tournament.SetSlots)
                {
                    slot.User1 = null;
                    slot.User2 = null;
                }
            }

            void AssignToSlots()
            {
                var firstRound = tournament.GetRound(Round.Brackets.Winners, 0);
                firstRound.SetSlots.Sort((x, y) => x.Identifier.CompareTo(y.Identifier));

                var slotIndex = 0;
                for(var i = 0; i < users.Count; i += 2)
                {
                    var p1 = users[i];
                    var p2 = users[i + 1];
                    var slot = firstRound.SetSlots[slotIndex];

                    if(p1 == TournamentUser.NullUser)
                    {
                        MoveUserPastBye(slot, p2);
                    }
                    else if(p2 == TournamentUser.NullUser)
                    {
                        MoveUserPastBye(slot, p1);
                    }
                    else
                    {
                        slot.User1 = p1;
                        slot.User2 = p2;
                    }

                    ++slotIndex;
                }
            }

            void MoveUserPastBye(SetSlot slot, TournamentUser user)
            {
                slot.IsBye = true;

                var nextSlot = tournament.SetSlots.First(ss => ss.Identifier == slot.WinSlotIdentifier);
                if(slot.Identifier == nextSlot.P1Game)
                {
                    nextSlot.User1 = user;
                }
                else
                {
                    nextSlot.User2 = user;
                }
            }

            void MarkLoserFirstRoundByes()
            {
                var round = tournament.GetRound(Round.Brackets.Losers, 0);
                foreach(var setSlot in round.SetSlots)
                {
                    if(tournament.SetSlots.First(ss => ss.Identifier == setSlot.P1Game).IsBye)
                    {
                        setSlot.IsBye = true;
                    }
                    else if(tournament.SetSlots.First(ss => ss.Identifier == setSlot.P2Game).IsBye)
                    {
                        setSlot.IsBye = true;
                    }
                }
            }
        }

        private static void SortSeeds(List<TournamentUser> competitors)
        {
            competitors.Sort((x, y) => x.Seed.CompareTo(y.Seed));

            for(var i = 0; i < competitors.Count; i++)
            {
                competitors[i].Seed = i + 1;
            }
        }

        private static Set CreateSet(Tournament tournament, SetSlot slot, TournamentUser p1, TournamentUser p2, DateTime dueDate)
        {
            return new Set(tournament.LeagueID, p1.LeagueUserID, p2.LeagueUserID, dueDate, tournament.SeasonID, p1.SeasonLeagueUserID, p2.SeasonLeagueUserID)
            {
                Tournament = tournament,
                SetSlot = slot,
                TournamentPlayer1 = p1,
                TournamentPlayer2 = p2,
                TournamentUser1ID = p1.ID,
                TournamentUser2ID = p2.ID,
            };
        }
    }
}