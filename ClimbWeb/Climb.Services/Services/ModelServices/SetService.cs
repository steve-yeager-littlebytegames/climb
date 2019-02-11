using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Climb.Data;
using Climb.Exceptions;
using Climb.Models;
using Climb.Requests.Sets;
using Microsoft.EntityFrameworkCore;

namespace Climb.Services.ModelServices
{
    public class SetService : ISetService
    {
        private readonly ApplicationDbContext dbContext;
        private readonly ISeasonService seasonService;
        private readonly IDateService dateService;

        public SetService(ApplicationDbContext dbContext, ISeasonService seasonService, IDateService dateService)
        {
            this.dbContext = dbContext;
            this.seasonService = seasonService;
            this.dateService = dateService;
        }

        public async Task<SetRequest> RequestSetAsync(int requesterID, int challengedID, string message)
        {
            if(!await dbContext.LeagueUsers.AnyAsync(lu => lu.ID == requesterID))
            {
                throw new NotFoundException(typeof(LeagueUser), requesterID);
            }

            if(!await dbContext.LeagueUsers.AnyAsync(lu => lu.ID == challengedID))
            {
                throw new NotFoundException(typeof(LeagueUser), challengedID);
            }

            var requester = await dbContext.LeagueUsers
                .Include(lu => lu.League).AsNoTracking()
                .FirstOrDefaultAsync(lu => lu.ID == requesterID);

            var setRequest = new SetRequest
            {
                LeagueID = requester.LeagueID,
                RequesterID = requesterID,
                ChallengedID = challengedID,
                DateCreated = dateService.Now,
                Message = message,
            };
            dbContext.Add(setRequest);
            await dbContext.SaveChangesAsync();

            return setRequest;
        }

        public async Task<SetRequest> RespondToSetRequestAsync(int requestID, bool accepted)
        {
            var setRequest = await dbContext.SetRequests
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(sr => sr.ID == requestID);
            if(setRequest == null)
            {
                throw new NotFoundException(typeof(SetRequest), requestID);
            }

            if(!setRequest.IsOpen)
            {
                throw new BadRequestException(nameof(requestID), $"Set Request {requestID} has already been closed.");
            }

            dbContext.Update(setRequest);
            setRequest.IsOpen = false;

            if(accepted)
            {
                var dueDate = dateService.Now.AddDays(7);
                var set = new Set(setRequest.LeagueID, setRequest.RequesterID, setRequest.ChallengedID, dueDate);

                dbContext.Add(set);
                setRequest.Set = set;
            }

            await dbContext.SaveChangesAsync();

            return setRequest;
        }

        public async Task<Set> Update(int setID, IReadOnlyList<MatchForm> matchForms)
        {
            var set = await dbContext.Sets
                .Include(s => s.Player1)
                .Include(s => s.Player2)
                .Include(s => s.Matches).ThenInclude(m => m.MatchCharacters)
                .FirstOrDefaultAsync(s => s.ID == setID);
            if(set == null)
            {
                throw new NotFoundException(typeof(Set), setID);
            }

            if(set.Matches.Count > 0)
            {
                dbContext.RemoveRange(set.Matches.SelectMany(m => m.MatchCharacters));
                dbContext.RemoveRange(set.Matches);
                await dbContext.SaveChangesAsync();
                set.Matches.Clear();
            }
            else
            {
                set.Player1.SetCount++;
                set.Player2.SetCount++;
            }

            dbContext.Update(set);

            set.IsComplete = true;
            set.Player1Score = set.Player2Score = 0;
            set.UpdatedDate = dateService.Now;

            for(var i = 0; i < matchForms.Count; i++)
            {
                var matchForm = matchForms[i];
                var match = new Match(setID, i, matchForm.Player1Score, matchForm.Player2Score, matchForm.StageID);
                dbContext.Matches.Add(match);

                for(var j = 0; j < matchForm.Player1Characters.Length; j++)
                {
                    AddCharacter(match, matchForm.Player1Characters[j], set.Player1ID, set.UpdatedDate.Value);
                    AddCharacter(match, matchForm.Player2Characters[j], set.Player2ID, set.UpdatedDate.Value);
                }

                if(matchForm.Player1Score > matchForm.Player2Score)
                {
                    ++set.Player1Score;
                }
                else
                {
                    ++set.Player2Score;
                }
            }

            await dbContext.SaveChangesAsync();

            if(set.SeasonID != null)
            {
                await seasonService.PlaySet(setID);
                await seasonService.UpdateRanksAsync(set.SeasonID.Value);
            }

            return set;

            void AddCharacter(Match match, int characterID, int playerID, DateTime createdDate)
            {
                var character = new MatchCharacter(match.ID, characterID, playerID, createdDate);
                dbContext.MatchCharacters.Add(character);
            }
        }

        public Set CreateTournamentSet(Tournament tournament, TournamentUser p1, TournamentUser p2, DateTime dueDate)
        {
            return new Set(tournament.LeagueID, p1.LeagueUserID, p2.LeagueUserID, dueDate, tournament.SeasonID, p1.SeasonLeagueUserID, p2.SeasonLeagueUserID)
            {
                Tournament = tournament,
                TournamentPlayer1 = p1,
                TournamentPlayer2 = p2,
            };
        }
    }
}