using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Climb.Attributes;
using Climb.Data;
using Climb.Models;
using Climb.Requests.Seasons;
using Climb.Services.ModelServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Climb.API
{
    public class SeasonApi : BaseApi<SeasonApi>
    {
        private readonly ApplicationDbContext dbContext;
        private readonly ISeasonService seasonService;

        public SeasonApi(ILogger<SeasonApi> logger, ApplicationDbContext dbContext, ISeasonService seasonService)
            : base(logger)
        {
            this.dbContext = dbContext;
            this.seasonService = seasonService;
        }

        [HttpGet("/api/v1/seasons/{seasonID:int}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(Season))]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(string))]
        public async Task<IActionResult> Get(int seasonID)
        {
            var season = await dbContext.Seasons
                .Include(s => s.Participants).ThenInclude(slu => slu.LeagueUser).AsNoTracking()
                .Include(s => s.Sets).AsNoTracking()
                .Include(s => s.League).AsNoTracking()
                .FirstOrDefaultAsync(s => s.ID == seasonID);
            if(season == null)
            {
                return CodeResultAndLog(HttpStatusCode.NotFound, $"No Season with ID '{seasonID}' found.");
            }

            return CodeResult(HttpStatusCode.OK, season);
        }

        [HttpGet("/api/v1/seasons/sets/{seasonID:int}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(Set[]))]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(string))]
        public async Task<IActionResult> Sets(int seasonID)
        {
            var season = await dbContext.Seasons
                .Include(s => s.Sets).AsNoTracking()
                .FirstOrDefaultAsync(s => s.ID == seasonID);
            if(season == null)
            {
                return CodeResultAndLog(HttpStatusCode.NotFound, $"No Season with ID '{seasonID}' found.");
            }

            return CodeResult(HttpStatusCode.OK, season.Sets);
        }

        [HttpGet("/api/v1/seasons/participants/{seasonID:int}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(LeagueUser[]))]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(string))]
        public async Task<IActionResult> Participants(int seasonID)
        {
            var season = await dbContext.Seasons
                .Include(s => s.Participants).ThenInclude(slu => slu.LeagueUser).AsNoTracking()
                .FirstOrDefaultAsync(s => s.ID == seasonID);
            if(season == null)
            {
                return CodeResultAndLog(HttpStatusCode.NotFound, $"No Season with ID '{seasonID}' found.");
            }

            var participants = season.Participants.Select(slu => slu.LeagueUser);
            return CodeResult(HttpStatusCode.OK, participants);
        }

        [HttpGet("/api/v1/seasons")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(Season[]))]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(string), "Can't find league.")]
        public async Task<IActionResult> ListForLeague(int leagueID)
        {
            var league = await dbContext.Leagues
                .Include(l => l.Seasons).AsNoTracking()
                .FirstOrDefaultAsync(l => l.ID == leagueID);
            if(league == null)
            {
                return CodeResultAndLog(HttpStatusCode.NotFound, $"No League with ID '{leagueID}' found.");
            }

            return CodeResult(HttpStatusCode.OK, league.Seasons);
        }

        [HttpPost("/api/v1/seasons/create")]
        [SwaggerResponse(HttpStatusCode.Created, typeof(Season))]
        [SwaggerResponse(HttpStatusCode.BadRequest, typeof(string), "Start and end date issues.")]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(string), "Can't find league.")]
        public async Task<IActionResult> Create(CreateRequest request)
        {
            try
            {
                var season = await seasonService.Create(request.LeagueID, request.StartDate, request.EndDate);
                return CodeResultAndLog(HttpStatusCode.Created, season, "Season created.");
            }
            catch(Exception exception)
            {
                return GetExceptionResult(exception, request);
            }
        }

        [HttpPost("/api/v1/seasons/start")]
        [SwaggerResponse(HttpStatusCode.Created, typeof(Set[]))]
        public async Task<IActionResult> Start(int seasonID)
        {
            try
            {
                var sets = await seasonService.GenerateSchedule(seasonID);
                return CodeResultAndLog(HttpStatusCode.Created, sets, "Schedule created.");
            }
            catch(Exception exception)
            {
                return GetExceptionResult(exception, new {seasonID});
            }
        }

        [HttpPost("/api/v1/seasons/end")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(Season))]
        public async Task<IActionResult> End(int seasonID)
        {
            try
            {
                var season = await seasonService.End(seasonID);
                return CodeResultAndLog(HttpStatusCode.OK, season, "Season ended.");
            }
            catch(Exception exception)
            {
                return GetExceptionResult(exception, new {seasonID});
            }
        }

        [HttpPost("/api/v1/seasons/leave")]
        [SwaggerResponse(HttpStatusCode.OK, null)]
        public async Task<IActionResult> Leave(int participantID)
        {
            try
            {
                var season = await seasonService.LeaveAsync(participantID);
                return CodeResultAndLog(HttpStatusCode.OK, season, "Season ended.");
            }
            catch(Exception exception)
            {
                return GetExceptionResult(exception, new {participantID});
            }
        }
    }
}