using System;
using System.Collections.Generic;
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

        public LeagueService(ApplicationDbContext dbContext, IPointService pointService)
        {
            this.dbContext = dbContext;
            this.pointService = pointService;
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

            var league = new League(gameID, name, adminID);
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
                leagueUser = new LeagueUser(leagueID, userID) {JoinDate = DateTime.UtcNow};
                dbContext.Add(leagueUser);
            }

            leagueUser.DisplayName = user.UserName;

            await dbContext.SaveChangesAsync();

            return leagueUser;
        }

        public async Task<League> UpdateStandings(int leagueID)
        {
            var league = await dbContext.Leagues
                .Include(l => l.Members)
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
                    // TODO: beginner multiplier
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
                for(var i = 0; i < activeMembers.Count; i++)
                {
                    var member = activeMembers[i];
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

            var createdDate = DateTime.Now;
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
    }
}