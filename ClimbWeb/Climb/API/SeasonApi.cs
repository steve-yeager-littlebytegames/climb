using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Climb.Attributes;
using Climb.Data;
using Climb.Requests.Seasons;
using Climb.Responses.Models;
using Climb.Responses.Sets;
using Climb.Services;
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
        private readonly ICdnService cdnService;

        public SeasonApi(ILogger<SeasonApi> logger, ApplicationDbContext dbContext, ISeasonService seasonService, ICdnService cdnService)
            : base(logger)
        {
            this.dbContext = dbContext;
            this.seasonService = seasonService;
            this.cdnService = cdnService;
        }

        [HttpGet("/api/v1/seasons/{seasonID:int}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(SeasonDto))]
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

            var dto = new SeasonDto(season);
            return CodeResult(HttpStatusCode.OK, dto);
        }

        [HttpGet("/api/v1/seasons/sets/{seasonID:int}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(IEnumerable<SetDto>))]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(string))]
        public async Task<IActionResult> Sets(int seasonID)
        {
            var season = await dbContext.Seasons
                .Include(s => s.League).AsNoTracking()
                .Include(s => s.Sets).ThenInclude(s => s.Matches).AsNoTracking()
                .Include(s => s.Sets).ThenInclude(s => s.League).AsNoTracking()
                .FirstOrDefaultAsync(s => s.ID == seasonID);
            if(season == null)
            {
                return CodeResultAndLog(HttpStatusCode.NotFound, $"No Season with ID '{seasonID}' found.");
            }

            var dtos = season.Sets.Select(s => SetDto.Create(s, season.League.GameID));
            return CodeResult(HttpStatusCode.OK, dtos);
        }

        [HttpGet("/api/v1/seasons/participants/{seasonID:int}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(IEnumerable<SeasonLeagueUserDto>))]
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

            var dtos = season.Participants.Select(slu => new SeasonLeagueUserDto(slu, cdnService));
            return CodeResult(HttpStatusCode.OK, dtos);
        }

        [HttpGet("/api/v1/seasons")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(IEnumerable<SeasonDto>))]
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

            var dtos = league.Seasons.Select(s => new SeasonDto(s));
            return CodeResult(HttpStatusCode.OK, dtos);
        }

        [HttpPost("/api/v1/seasons/create")]
        [SwaggerResponse(HttpStatusCode.Created, typeof(SeasonDto))]
        [SwaggerResponse(HttpStatusCode.BadRequest, typeof(string), "Start and end date issues.")]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(string), "Can't find league.")]
        public async Task<IActionResult> Create(CreateRequest request)
        {
            try
            {
                var season = await seasonService.Create(request.LeagueID, request.StartDate, request.EndDate);
                var dto = new SeasonDto(season);
                return CodeResultAndLog(HttpStatusCode.Created, dto, "Season created.");
            }
            catch(Exception exception)
            {
                return GetExceptionResult(exception, request);
            }
        }

        [HttpPost("/api/v1/seasons/start")]
        [SwaggerResponse(HttpStatusCode.Created, typeof(IEnumerable<SetDto>))]
        public async Task<IActionResult> Start(int seasonID)
        {
            try
            {
                var season = await seasonService.GenerateSchedule(seasonID);
                var dtos = season.Sets.Select(s => SetDto.Create(s, season.League.GameID));
                return CodeResultAndLog(HttpStatusCode.Created, dtos, "Schedule created.");
            }
            catch(Exception exception)
            {
                return GetExceptionResult(exception, new {seasonID});
            }
        }

        [HttpPost("/api/v1/seasons/end")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(SeasonDto))]
        public async Task<IActionResult> End(int seasonID)
        {
            try
            {
                var season = await seasonService.End(seasonID);
                var dto = new SeasonDto(season);
                return CodeResultAndLog(HttpStatusCode.OK, dto, "Season ended.");
            }
            catch(Exception exception)
            {
                return GetExceptionResult(exception, new {seasonID});
            }
        }

        [HttpPost("/api/v1/seasons/leave")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(SeasonDto))]
        public async Task<IActionResult> Leave(int participantID)
        {
            try
            {
                var season = await seasonService.LeaveAsync(participantID);
                var dto = new SeasonDto(season);
                return CodeResultAndLog(HttpStatusCode.OK, dto, "Left season.");
            }
            catch(Exception exception)
            {
                return GetExceptionResult(exception, new {participantID});
            }
        }

        [HttpPost("/api/v1/seasons/update-ranks")]
        public async Task<IActionResult> UpdateStandings(int seasonID)
        {
            try
            {
                var season = await seasonService.UpdateRanksAsync(seasonID);
                return CodeResultAndLog(HttpStatusCode.OK, season.Participants, $"Season {seasonID} standings updated.");
            }
            catch(Exception exception)
            {
                return GetExceptionResult(exception, new {seasonID});
            }
        }
    }
}