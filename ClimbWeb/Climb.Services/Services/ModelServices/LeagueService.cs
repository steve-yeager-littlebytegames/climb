﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Climb.Data;
using Climb.Exceptions;
using Climb.Models;
using Microsoft.EntityFrameworkCore;
using MoreLinq;

namespace Climb.Services.ModelServices
{
    public class LeagueService : ILeagueService
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IPointService pointService;
        private readonly ISeasonService seasonService;
        private readonly ISetService setService;
        private readonly IDateService dateService;

        public LeagueService(ApplicationDbContext dbContext, IPointService pointService, ISeasonService seasonService, ISetService setService, IDateService dateService)
        {
            this.dbContext = dbContext;
            this.pointService = pointService;
            this.seasonService = seasonService;
            this.setService = setService;
            this.dateService = dateService;
        }

        public async Task<League> Create(string name, int gameID, string adminID)
        {
            if(!await dbContext.Games.AnyAsync(g => g.ID == gameID))
            {
                throw new NotFoundException(typeof(Game), gameID);
            }

            if(await dbContext.Leagues.AnyAsync(l => l.Name == name))
            {
                throw new ConflictException(typeof(League), nameof(League.Name), name);
            }

            if(!await dbContext.Users.AnyAsync(u => u.Id == adminID))
            {
                throw new NotFoundException(typeof(ApplicationUser), adminID);
            }

            var league = new League(gameID, name, adminID, dateService.Now);
            dbContext.Add(league);
            await dbContext.SaveChangesAsync();

            await Join(league.ID, adminID);

            return league;
        }

        public async Task<LeagueUser> Join(int leagueID, string userID)
        {
            if(!await dbContext.Leagues.AnyAsync(l => l.ID == leagueID))
            {
                throw new NotFoundException(typeof(League), leagueID);
            }

            var user = await dbContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userID);
            if(user == null)
            {
                throw new NotFoundException(typeof(ApplicationUser), userID);
            }

            var leagueUser = await dbContext.LeagueUsers
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(lu => lu.LeagueID == leagueID && lu.UserID == userID);
            if(leagueUser != null)
            {
                leagueUser.HasLeft = false;
            }
            else
            {
                leagueUser = new LeagueUser(leagueID, userID)
                {
                    JoinDate = dateService.Now,
                    Points = League.StartingPoints
                };
                dbContext.Add(leagueUser);
            }

            leagueUser.DisplayName = user.UserName;

            await dbContext.SaveChangesAsync();

            return leagueUser;
        }

        public async Task<LeagueUser> Leave(int leagueUserID)
        {
            var member = await dbContext.LeagueUsers
                .IgnoreQueryFilters()
                .Include(lu => lu.Seasons).ThenInclude(slu => slu.Season)
                .FirstOrDefaultAsync(lu => lu.ID == leagueUserID);
            if(member == null)
            {
                throw new NotFoundException(typeof(LeagueUser), leagueUserID);
            }

            if(member.HasLeft)
            {
                return member;
            }

            dbContext.Update(member);
            member.HasLeft = true;

            foreach(var participant in member.Seasons.Where(s => !s.Season.IsComplete))
            {
                await seasonService.LeaveAsync(participant.ID);
            }

            await DeleteChallenges();
            await DeleteNonSeasonSets();

            await dbContext.SaveChangesAsync();

            return member;

            async Task DeleteChallenges()
            {
                var requests = await dbContext.SetRequests
                    .Where(sr => sr.IsOpen && (sr.RequesterID == member.ID || sr.ChallengedID == member.ID))
                    .ToArrayAsync();
                foreach(var request in requests)
                {
                    if(request.ChallengedID == member.ID)
                    {
                        await setService.RespondToSetRequestAsync(request.ID, false);
                    }
                    else
                    {
                        dbContext.Remove(request);
                    }
                }
            }

            async Task DeleteNonSeasonSets()
            {
                var sets = await dbContext.Sets
                    .Where(s => !s.IsComplete && s.SeasonID == null && s.IsPlaying(member.ID))
                    .ToArrayAsync();
                foreach(var set in sets)
                {
                    dbContext.Remove(set);
                }
            }
        }

        public async Task<League> UpdateStandings(int leagueID)
        {
            var league = await dbContext.Leagues
                .Include(l => l.Members).IgnoreQueryFilters()
                .FirstOrDefaultAsync(l => l.ID == leagueID);
            if(league == null)
            {
                throw new NotFoundException(typeof(League), leagueID);
            }

            var playedSets = await dbContext.Sets.Where(s => s.LeagueID == leagueID
                                                             && s.IsComplete
                                                             && !s.IsLocked).ToArrayAsync();

            dbContext.Sets.UpdateRange(playedSets);
            dbContext.UpdateRange(league.Members);

            UpdatePoints();
            UpdateRanks();

            await dbContext.SaveChangesAsync();

            return league;

            void UpdatePoints()
            {
                var pointsPerMember = new Dictionary<int, int>();

                foreach(var set in playedSets)
                {
                    var player1Won = set.WinnerID == set.Player1ID;
                    var (p1Points, p2Points) = pointService.CalculatePointDeltas(set.Player1.Points, set.Player2.Points, player1Won);
                    if(!pointsPerMember.ContainsKey(set.Player1ID))
                    {
                        pointsPerMember.Add(set.Player1ID, 0);
                    }

                    if(!pointsPerMember.ContainsKey(set.Player2ID))
                    {
                        pointsPerMember.Add(set.Player2ID, 0);
                    }

                    pointsPerMember[set.Player1ID] += p1Points;
                    pointsPerMember[set.Player2ID] += p2Points;

                    set.IsLocked = true;
                }

                foreach(var member in league.Members)
                {
                    if(pointsPerMember.TryGetValue(member.ID, out var eloDelta))
                    {
                        member.Points += eloDelta;
                    }
                }
            }

            void UpdateRanks()
            {
                var activeMembers = league.Members.OrderByDescending(lu => lu.Points).ToList();

                var rank = 0;
                var rankedMembers = 0;
                var lastPoints = -1;
                foreach(var member in activeMembers)
                {
                    if(league.IsMemberNew(member))
                    {
                        member.Rank = 0;
                        continue;
                    }

                    if(member.Points != lastPoints)
                    {
                        lastPoints = member.Points;
                        rank = rankedMembers + 1;
                    }

                    ++rankedMembers;
                    if(member.Rank == 0)
                    {
                        member.RankTrend = RankTrends.Up;
                    }
                    else if(member.Rank < rank)
                    {
                        member.RankTrend = RankTrends.Down;
                    }
                    else if(member.Rank > rank)
                    {
                        member.RankTrend = RankTrends.Up;
                    }
                    else
                    {
                        member.RankTrend = RankTrends.None;
                    }
                    member.Rank = rank;
                }
            }
        }

        public async Task<IReadOnlyList<RankSnapshot>> TakeSnapshots(int leagueID)
        {
            var league = await dbContext.Leagues
                .Include(l => l.Members).ThenInclude(lu => lu.RankSnapshots)
                .FirstOrDefaultAsync(l => l.ID == leagueID);
            if(league == null)
            {
                throw new NotFoundException(typeof(League), leagueID);
            }

            var createdDate = dateService.Now;
            var rankSnapshots = new RankSnapshot[league.Members.Count];
            for(var i = 0; i < league.Members.Count; ++i)
            {
                var member = league.Members[i];
                RankSnapshot lastSnapshot = null;
                if(member.RankSnapshots?.Count > 0)
                {
                    lastSnapshot = member.RankSnapshots.MaxBy(rs => rs.CreatedDate).Take(1).First();
                }

                var rankDelta = member.Rank - (lastSnapshot?.Rank ?? 0);
                var pointsDelta = member.Points - (lastSnapshot?.Points ?? 0);
                var rankSnapshot = new RankSnapshot
                {
                    LeagueUserID = member.ID,
                    Rank = member.Rank,
                    Points = member.Points,
                    DeltaRank = rankDelta,
                    DeltaPoints = pointsDelta,
                    CreatedDate = createdDate
                };
                rankSnapshots[i] = rankSnapshot;

                if(member.IsNewcomer && !league.IsMemberNew(member))
                {
                    member.IsNewcomer = false;
                    dbContext.Update(member);
                }
            }

            dbContext.RankSnapshots.AddRange(rankSnapshots);
            await dbContext.SaveChangesAsync();

            return rankSnapshots;
        }

        public async Task<List<Character>> GetUsersRecentCharactersAsync(int leagueUserID, int characterCount)
        {
            const int charactersToPull = 20;
            const int requiredCount = 1;

            if(characterCount < requiredCount)
            {
                throw new BadRequestException(nameof(characterCount), $"Min characters to request is {requiredCount}.");
            }

            if(!await dbContext.LeagueUsers.AnyAsync(lu => lu.ID == leagueUserID))
            {
                throw new NotFoundException(typeof(LeagueUser), leagueUserID);
            }

            var matchCharacters = await dbContext.MatchCharacters
                .Where(mc => mc.LeagueUserID == leagueUserID)
                .OrderByDescending(mc => mc.CreatedDate)
                .Take(charactersToPull)
                .Include(mc => mc.Character)
                .ToArrayAsync();

            var characterMap = new Dictionary<int, Character>(matchCharacters.Length);

            var characterUsage = new Dictionary<int, int>(charactersToPull);
            foreach(var matchCharacter in matchCharacters)
            {
                if(characterUsage.ContainsKey(matchCharacter.CharacterID))
                {
                    ++characterUsage[matchCharacter.CharacterID];
                }
                else
                {
                    characterMap[matchCharacter.CharacterID] = matchCharacter.Character;
                    characterUsage[matchCharacter.CharacterID] = 1;
                }
            }

            return characterUsage
                .OrderByDescending(x => x.Value)
                .Take(characterCount)
                .Select(x => characterMap[x.Key])
                .ToList();
        }
    }
}