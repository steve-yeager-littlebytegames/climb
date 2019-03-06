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
    [Route("/api/v1/seasons")]
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

        [HttpGet("{id:int}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(SeasonDto))]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(string))]
        public async Task<IActionResult> Get(int id)
        {
            var season = await dbContext.Seasons
                .Include(s => s.Participants).ThenInclude(slu => slu.LeagueUser).AsNoTracking()
                .Include(s => s.Sets).AsNoTracking()
                .Include(s => s.League).AsNoTracking()
                .FirstOrDefaultAsync(s => s.ID == id);
            if(season == null)
            {
                return GetCodeResult(HttpStatusCode.NotFound, $"No Season with ID '{id}' found.");
            }

            var dto = new SeasonDto(season);
            return CodeResult(HttpStatusCode.OK, dto);
        }

        [HttpPost("")]
        [SwaggerResponse(HttpStatusCode.Created, typeof(SeasonDto))]
        [SwaggerResponse(HttpStatusCode.BadRequest, typeof(string), "Start and end date issues.")]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(string), "Can't find league.")]
        public async Task<IActionResult> Create(CreateRequest request)
        {
            try
            {
                var season = await seasonService.Create(request.LeagueID, request.StartDate, request.EndDate);
                var dto = new SeasonDto(season);
                return GetCodeResult(HttpStatusCode.Created, dto, "Season created.");
            }
            catch(Exception exception)
            {
                return GetExceptionResult(exception, request);
            }
        }

        [HttpGet("{id:int}/sets")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(SetDto[]))]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(string))]
        public async Task<IActionResult> Sets(int id)
        {
            var season = await dbContext.Seasons
                .Include(s => s.League).AsNoTracking()
                .Include(s => s.Sets).ThenInclude(s => s.Matches).AsNoTracking()
                .Include(s => s.Sets).ThenInclude(s => s.League).AsNoTracking()
                .FirstOrDefaultAsync(s => s.ID == id);
            if(season == null)
            {
                return GetCodeResult(HttpStatusCode.NotFound, $"No Season with ID '{id}' found.");
            }

            var dtos = season.Sets.Select(s => SetDto.Create(s, season.League.GameID));
            return CodeResult(HttpStatusCode.OK, dtos);
        }

        [HttpGet("{id:int}/participants")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(IEnumerable<SeasonLeagueUserDto>))]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(string))]
        public async Task<IActionResult> Participants(int id)
        {
            var season = await dbContext.Seasons
                .Include(s => s.Participants).ThenInclude(slu => slu.LeagueUser).AsNoTracking()
                .FirstOrDefaultAsync(s => s.ID == id);
            if(season == null)
            {
                return GetCodeResult(HttpStatusCode.NotFound, $"No Season with ID '{id}' found.");
            }

            var dtos = season.Participants.Select(slu => new SeasonLeagueUserDto(slu, cdnService));
            return CodeResult(HttpStatusCode.OK, dtos);
        }

        [HttpPost("{id:int}/start")]
        [SwaggerResponse(HttpStatusCode.Created, typeof(IEnumerable<SetDto>))]
        public async Task<IActionResult> Start(int id)
        {
            try
            {
                var season = await seasonService.GenerateSchedule(id);
                var dtos = season.Sets.Select(s => SetDto.Create(s, season.League.GameID));
                return GetCodeResult(HttpStatusCode.Created, dtos, "Schedule created.");
            }
            catch(Exception exception)
            {
                return GetExceptionResult(exception, new {seasonID = id});
            }
        }

        [HttpPost("{id:int}/end")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(SeasonDto))]
        public async Task<IActionResult> End(int id)
        {
            try
            {
                var season = await seasonService.End(id);
                var dto = new SeasonDto(season);
                return GetCodeResult(HttpStatusCode.OK, dto, "Season ended.");
            }
            catch(Exception exception)
            {
                return GetExceptionResult(exception, new {seasonID = id});
            }
        }

        [HttpPost("{id:int}/update-ranks")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(string))]
        public async Task<IActionResult> UpdateStandings(int id)
        {
            try
            {
                var season = await seasonService.UpdateRanksAsync(id);
                return GetCodeResult(HttpStatusCode.OK, season.Participants, $"Season {id} standings updated.");
            }
            catch(Exception exception)
            {
                return GetExceptionResult(exception, new {seasonID = id});
            }
        }

        [HttpPost("leave/{participantID:int}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(SeasonDto))]
        public async Task<IActionResult> Leave(int participantID)
        {
            try
            {
                var season = await seasonService.LeaveAsync(participantID);
                var dto = new SeasonDto(season);
                return GetCodeResult(HttpStatusCode.OK, dto, "Left season.");
            }
            catch(Exception exception)
            {
                return GetExceptionResult(exception, new {participantID});
            }
        }
    }
}