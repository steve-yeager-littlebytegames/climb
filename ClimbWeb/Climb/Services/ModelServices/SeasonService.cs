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

        public SeasonService(ApplicationDbContext dbContext, IScheduleFactory scheduleFactory, ISeasonPointCalculator pointCalculator, ITieBreakerFactory tieBreakerFactory)
        {
            this.dbContext = dbContext;
            this.scheduleFactory = scheduleFactory;
            this.pointCalculator = pointCalculator;

            tieBreaker = tieBreakerFactory.Create();
        }

        public async Task<Season> Create(int leagueID, DateTime start, DateTime end)
        {
            if(start < DateTime.Now)
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

            await scheduleFactory.GenerateScheduleAsync(season, dbContext);
            season.League.ActiveSeasonID = seasonID;

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

            dbContext.UpdateRange(participant.P1Sets);
            dbContext.UpdateRange(participant.P2Sets);
            participant.P1Sets.ForEach(s => s.IsDisabled = true);
            participant.P2Sets.ForEach(s => s.IsDisabled = true);

            if(participant.Season.IsActive)
            {
                await UpdateRanksAsync(participant.SeasonID);
            }

            await dbContext.SaveChangesAsync();

            return participant.Season;
        }
    }
}