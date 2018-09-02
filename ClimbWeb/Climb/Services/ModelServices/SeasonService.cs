using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Climb.Core.TieBreakers;
using Climb.Data;
using Climb.Exceptions;
using Climb.Models;
using Microsoft.EntityFrameworkCore;

namespace Climb.Services.ModelServices
{
    public class SeasonService : ISeasonService
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IScheduleFactory scheduleFactory;
        private readonly ISeasonPointCalculator pointCalculator;
        private readonly ITieBreaker tieBreaker;
        private readonly IDateService dateService;

        public SeasonService(ApplicationDbContext dbContext, IScheduleFactory scheduleFactory, ISeasonPointCalculator pointCalculator, ITieBreakerFactory tieBreakerFactory, IDateService dateService)
        {
            this.dbContext = dbContext;
            this.scheduleFactory = scheduleFactory;
            this.pointCalculator = pointCalculator;
            this.dateService = dateService;

            tieBreaker = tieBreakerFactory.Create(dateService.Now);
        }

        public async Task<Season> Create(int leagueID, DateTime start, DateTime end)
        {
            if(start < dateService.Now)
            {
                throw new BadRequestException(nameof(start), "Start date can't be in the past.");
            }

            if(end <= start)
            {
                throw new BadRequestException(nameof(end), "End date must be after the start date.");
            }

            var league = await dbContext.Leagues
                .Include(l => l.Members).AsNoTracking()
                .FirstOrDefaultAsync(l => l.ID == leagueID);
            if(league == null)
            {
                throw new NotFoundException(typeof(League), leagueID);
            }

            var seasonCount = await dbContext.Seasons.CountAsync(s => s.LeagueID == leagueID);

            var season = new Season(leagueID, seasonCount, start, end);
            dbContext.Add(season);

            var participants = league.Members.Select(lu => new SeasonLeagueUser(season.ID, lu.ID, lu.UserID));
            dbContext.AddRange(participants);

            await dbContext.SaveChangesAsync();

            return season;
        }

        public async Task<Season> GenerateSchedule(int seasonID)
        {
            var season = await dbContext.Seasons
                .Include(s => s.League)
                .Include(s => s.Sets)
                .Include(s => s.Participants).AsNoTracking()
                .FirstOrDefaultAsync(s => s.ID == seasonID);
            if(season == null)
            {
                throw new NotFoundException(typeof(Season), seasonID);
            }

            var sets = scheduleFactory.GenerateSchedule(season.StartDate, season.EndDate, season.Participants);
            dbContext.AddRange(sets);
            await dbContext.SaveChangesAsync();

            return season;
        }

        public async Task<Season> PlaySet(int setID)
        {
            var set = await dbContext.Sets
                .Include(s => s.Season).ThenInclude(s => s.Participants).ThenInclude(slu => slu.LeagueUser)
                .Include(s => s.Season).ThenInclude(s => s.Sets).ThenInclude(s => s.Player1)
                .Include(s => s.Season).ThenInclude(s => s.Sets).ThenInclude(s => s.Player2)
                .FirstOrDefaultAsync(s => s.ID == setID);
            dbContext.Update(set);

            UpdatePoints(set);

            await dbContext.SaveChangesAsync();

            return set.Season;
        }

        public async Task<Season> UpdateRanksAsync(int seasonID)
        {
            var season = await dbContext.Seasons
                .Include(s => s.Participants).IgnoreQueryFilters()
                .Include(s => s.Sets).ThenInclude(s => s.Player1)
                .Include(s => s.Sets).ThenInclude(s => s.Player2)
                .FirstOrDefaultAsync(s => s.ID == seasonID);
            dbContext.UpdateRange(season.Participants);

            BreakTies(season);

            var sortedParticipants = season.Participants
                .OrderByDescending(slu => slu.Points)
                .ThenByDescending(slu => slu.TieBreakerPoints)
                .ToArray();

            var rank = 1;
            var lastPoints = -1;
            foreach(var participant in sortedParticipants)
            {
                if(participant.HasLeft)
                {
                    participant.Standing = 0;
                    continue;
                }

                participant.Standing = rank;
                if(participant.Points != lastPoints)
                {
                    lastPoints = participant.Points;
                }

                ++rank;
            }

            return season;
        }

        private void UpdatePoints(Set set)
        {
            var winner = set.WinnerID == set.Player1ID ? set.SeasonPlayer1 : set.SeasonPlayer2;
            dbContext.Update(winner);
            var loser = set.LoserID == set.Player1ID ? set.SeasonPlayer1 : set.SeasonPlayer2;
            dbContext.Update(loser);

            var (winnerPointDelta, loserPointDelta) = pointCalculator.CalculatePointDeltas(winner, loser);

            set.Player1SeasonPoints = set.WinnerID == set.Player1ID ? winnerPointDelta : loserPointDelta;
            set.Player2SeasonPoints = set.WinnerID == set.Player2ID ? winnerPointDelta : loserPointDelta;
            winner.Points += winnerPointDelta;
            loser.Points += loserPointDelta;
        }

        private void BreakTies(Season season)
        {
            var playedSets = season.Sets.Where(s => s.IsComplete).ToArray();
            var setWins = new Dictionary<int, List<int>>();
            foreach(var participant in season.Participants)
            {
                participant.TieBreakerPoints = 0;
                setWins.Add(participant.ID, new List<int>());
            }

            foreach(var set in playedSets)
            {
                Debug.Assert(set.SeasonWinnerID != null, "set.SeasonWinnerID != null");
                Debug.Assert(set.SeasonLoserID != null, "set.SeasonLoserID != null");
                setWins[set.SeasonWinnerID.Value].Add(set.SeasonLoserID.Value);
            }

            season.Participants.Sort();

            var currentPoints = -1;
            var tiedParticipants = new Dictionary<int, Dictionary<IParticipant, ParticipantRecord>>();
            foreach(var seasonLeagueUser in season.Participants)
            {
                if(seasonLeagueUser.Points != currentPoints)
                {
                    currentPoints = seasonLeagueUser.Points;
                    tiedParticipants.Add(currentPoints, new Dictionary<IParticipant, ParticipantRecord>());
                }

                var record = new ParticipantRecord(seasonLeagueUser.LeagueUser.Points, seasonLeagueUser.LeagueUser.JoinDate);
                tiedParticipants[currentPoints].Add(seasonLeagueUser, record);
            }

            foreach(var tiedParticipantGroup in tiedParticipants)
            {
                if(tiedParticipantGroup.Value.Count <= 1)
                {
                    continue;
                }

                foreach(var participantRecord in tiedParticipantGroup.Value)
                {
                    participantRecord.Value.AddWins(setWins[participantRecord.Key.ID]);
                }

                tieBreaker.Break(tiedParticipantGroup.Value);
            }
        }

        public async Task<Season> End(int seasonID)
        {
            var season = await dbContext.Seasons
                .Include(s => s.League)
                .Include(s => s.Sets)
                .FirstOrDefaultAsync(s => s.ID == seasonID);
            if(season == null)
            {
                throw new NotFoundException(typeof(Season), seasonID);
            }

            if(season.IsComplete)
            {
                throw new BadRequestException($"Cannot end Season {seasonID} before it starts.");
            }

            if(!season.IsActive)
            {
                throw new BadRequestException($"Cannot end Season {seasonID} before it starts.");
            }

            dbContext.Update(season);
            season.IsComplete = true;
            season.IsActive = false;
            season.League.ActiveSeasonID = null;

            for(var i = season.Sets.Count - 1; i >= 0; i--)
            {
                var set = season.Sets[i];
                if (set.IsComplete)
                {
                    if(!set.IsLocked)
                    {
                        set.IsLocked = true;
                        dbContext.Update(set);
                    }
                }
                else
                {
                    dbContext.Remove(set);
                    season.Sets.RemoveAt(i);
                }
            }

            await dbContext.SaveChangesAsync();

            return season;
        }

        public async Task<Season> LeaveAsync(int participantID)
        {
            var participant = await dbContext.SeasonLeagueUsers
                .IgnoreQueryFilters()
                .Include(slu => slu.Season)
                .Include(slu => slu.P1Sets)
                .Include(slu => slu.P2Sets)
                .FirstOrDefaultAsync(slu => slu.ID == participantID);
            if(participant == null)
            {
                throw new NotFoundException(typeof(SeasonLeagueUser), participantID);
            }

            if(participant.Season.IsComplete)
            {
                throw new BadRequestException($"Can't leave season '{participant.SeasonID}' because it's already completed.");
            }

            dbContext.Update(participant);
            participant.HasLeft = true;
            participant.Standing = participant.Points = participant.TieBreakerPoints = 0;

            var sets = new List<Set>(participant.P1Sets.Count + participant.P2Sets.Count);
            sets.AddRange(participant.P1Sets.Where(s => !s.IsComplete));
            sets.AddRange(participant.P2Sets.Where(s => !s.IsComplete));
            sets.ForEach(s => s.Forfeit(participant.LeagueUserID));

            if(participant.Season.IsActive)
            {
                await UpdateRanksAsync(participant.SeasonID);
            }

            await dbContext.SaveChangesAsync();

            return participant.Season;
        }

        public async Task<Season> JoinAsync(int seasonID, string userID)
        {
            var season = await dbContext.Seasons
                .Include(s => s.Sets)
                .Include(s => s.Participants).IgnoreQueryFilters()
                .FirstOrDefaultAsync(s => s.ID == seasonID);
            if(season == null)
            {
                throw new NotFoundException(typeof(Season), seasonID);
            }

            if(season.IsActive)
            {
                throw new BadRequestException($"Can't join season '{seasonID}' because it's already started.");
            }

            if(!await dbContext.Users.AnyAsync(u => u.Id == userID))
            {
                throw new NotFoundException(typeof(ApplicationUser), userID);
            }

            var participant = season.Participants.FirstOrDefault(slu => slu.UserID == userID);
            if(participant != null)
            {
                if(participant.HasLeft)
                {
                    dbContext.Update(participant);
                    participant.HasLeft = false;
                }
                else
                {
                    return season;
                }
            }
            else
            {
                var leagueUser = await dbContext.LeagueUsers.FirstOrDefaultAsync(lu => lu.LeagueID == season.LeagueID && lu.UserID == userID);

                participant = new SeasonLeagueUser(seasonID, leagueUser.ID, userID);
                season.Participants.Add(participant);
                dbContext.Add(participant);
            }


            await dbContext.SaveChangesAsync();

            return season;
        }
    }
}