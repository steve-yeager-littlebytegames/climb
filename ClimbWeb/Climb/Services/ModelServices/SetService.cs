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

        public SetService(ApplicationDbContext dbContext, ISeasonService seasonService)
        {
            this.dbContext = dbContext;
            this.seasonService = seasonService;
        }

        public async Task<SetRequest> RequestSetAsync(int requesterID, int challengedID, string message)
        {
            if (!await dbContext.LeagueUsers.AnyAsync(lu => lu.ID == requesterID))
            {
                throw new NotFoundException(typeof(LeagueUser), requesterID);
            }
            if (!await dbContext.LeagueUsers.AnyAsync(lu => lu.ID == challengedID))
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
                DateCreated = DateTime.Now,
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
                var set = new Set
                {
                    LeagueID = setRequest.LeagueID,
                    Player1ID = setRequest.RequesterID,
                    Player2ID = setRequest.ChallengedID,
                };

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

            dbContext.Sets.Update(set);

            set.IsComplete = true;
            set.Player1Score = set.Player2Score = 0;

            for(var i = 0; i < matchForms.Count; i++)
            {
                var matchForm = matchForms[i];
                var match = new Match(setID, i, matchForm.Player1Score, matchForm.Player2Score, matchForm.StageID);
                dbContext.Matches.Add(match);

                for(var j = 0; j < matchForm.Player1Characters.Length; j++)
                {
                    AddCharacter(match, matchForm.Player1Characters[j], set.Player1ID);
                    AddCharacter(match, matchForm.Player2Characters[j], set.Player2ID);
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

            if (set.SeasonID != null)
            {
                await seasonService.UpdateStandings(setID);
            }

            return set;

            void AddCharacter(Match match, int characterID, int playerID)
            {
                var character = new MatchCharacter(match.ID, characterID, playerID);
                dbContext.MatchCharacters.Add(character);
            }
        }
    }
}