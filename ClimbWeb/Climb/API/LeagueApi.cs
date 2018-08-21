using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Climb.Attributes;
using Climb.Data;
using Climb.Models;
using Climb.Requests.Leagues;
using Climb.Responses.Models;
using Climb.Services.ModelServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Climb.API
{
    public class LeagueApi : BaseApi<LeagueApi>
    {
        private readonly ApplicationDbContext dbContext;
        private readonly ILeagueService leagueService;
        private readonly string adminKey;

        public LeagueApi(ILogger<LeagueApi> logger, ApplicationDbContext dbContext, ILeagueService leagueService, IConfiguration configuration)
            : base(logger)
        {
            this.dbContext = dbContext;
            this.leagueService = leagueService;
            adminKey = configuration["AdminKey"];
        }

        [HttpGet("/api/v1/leagues")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(List<League>))]
        public async Task<IActionResult> ListAll()
        {
            var leagues = await dbContext.Leagues.ToListAsync();

            return CodeResult(HttpStatusCode.OK, leagues);
        }

        [HttpGet("/api/v1/leagues/{leagueID:int}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(League))]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(string))]
        public async Task<IActionResult> Get(int leagueID)
        {
            var league = await dbContext.Leagues.FirstOrDefaultAsync(l => l.ID == leagueID);
            if(league == null)
            {
                return CodeResultAndLog(HttpStatusCode.NotFound, $"No League with ID '{leagueID}' found.");
            }

            return CodeResult(HttpStatusCode.OK, league);
        }

        [HttpPost("/api/v1/leagues/create")]
        [SwaggerResponse(HttpStatusCode.Created, typeof(League))]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(string), "Can't find game.")]
        [SwaggerResponse(HttpStatusCode.Conflict, typeof(string), "League name taken.")]
        public async Task<IActionResult> Create(CreateRequest request)
        {
            try
            {
                var league = await leagueService.Create(request.Name, request.GameID, request.AdminID);
                return CodeResult(HttpStatusCode.Created, league);
            }
            catch(Exception exception)
            {
                return GetExceptionResult(exception, request);
            }
        }

        [HttpPost("/api/v1/leagues/join")]
        [SwaggerResponse(HttpStatusCode.Created, typeof(LeagueUser))]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(string), "Can't find league.")]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(string), "Can't find user.")]
        public async Task<IActionResult> Join(JoinRequest request)
        {
            try
            {
                var leagueUser = await leagueService.Join(request.LeagueID, request.UserID);
                return CodeResultAndLog(HttpStatusCode.Created, leagueUser, "User joined league.");
            }
            catch(Exception exception)
            {
                return GetExceptionResult(exception, request);
            }
        }

        [HttpGet("/api/v1/leagues/user/{userID:int}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(LeagueUserDto))]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(string))]
        public async Task<IActionResult> GetUser(int userID)
        {
            var leagueUser = await dbContext.LeagueUsers.Include(lu => lu.User).AsNoTracking().FirstOrDefaultAsync(lu => lu.ID == userID);
            if(leagueUser == null)
            {
                return CodeResultAndLog(HttpStatusCode.NotFound, $"Could not find League User with ID '{userID}'.");
            }

            var response = new LeagueUserDto(leagueUser);
            return CodeResult(HttpStatusCode.OK, response);
        }

        [HttpGet("/api/v1/leagues/seasons/{leagueID:int}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(Season[]))]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(string), "Can't find league.")]
        public async Task<IActionResult> GetSeasons(int leagueID)
        {
            var league = await dbContext.Leagues.Include(l => l.Seasons).AsNoTracking().FirstOrDefaultAsync(l => l.ID == leagueID);
            if(league == null)
            {
                return CodeResultAndLog(HttpStatusCode.NotFound, $"No League with ID '{leagueID}' found.");
            }

            return CodeResult(HttpStatusCode.OK, league.Seasons);
        }

        [HttpPost("/api/v1/leagues/update-standings/{leagueID:int}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(League), "League power rankings have been updated.")]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(string), "Can't find league.")]
        public async Task<IActionResult> UpdateStandings(int leagueID, [FromHeader]string key)
        {
            if(key != adminKey)
            {
                return Unauthorized();
            }

            try
            {
                var league = await leagueService.UpdateStandings(leagueID);
                return Ok(league);
            }
            catch(Exception exception)
            {
                return GetExceptionResult(exception, new {leagueID});
            }
        }
    }
}