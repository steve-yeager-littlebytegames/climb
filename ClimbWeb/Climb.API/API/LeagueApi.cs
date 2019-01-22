using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Climb.Attributes;
using Climb.Data;
using Climb.Requests.Leagues;
using Climb.Responses.Models;
using Climb.Responses.Sets;
using Climb.Services;
using Climb.Services.ModelServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Climb.API
{
    [Route("/api/v1/leagues")]
    public class LeagueApi : BaseApi<LeagueApi>
    {
        private readonly ApplicationDbContext dbContext;
        private readonly ILeagueService leagueService;
        private readonly string adminKey;
        private readonly ICdnService cdnService;

        public LeagueApi(ILogger<LeagueApi> logger, ApplicationDbContext dbContext, ILeagueService leagueService, IConfiguration configuration, ICdnService cdnService)
            : base(logger)
        {
            this.dbContext = dbContext;
            this.leagueService = leagueService;
            this.cdnService = cdnService;
            adminKey = configuration["AdminKey"];
        }

        [HttpGet("")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(IEnumerable<LeagueDto>))]
        public async Task<IActionResult> GetAll()
        {
            var leagues = await dbContext.Leagues.ToListAsync();
            var dtos = leagues.Select(l => new LeagueDto(l));
            return CodeResult(HttpStatusCode.OK, dtos);
        }

        [HttpGet("{id:int}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(LeagueDto))]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(string))]
        public async Task<IActionResult> Get(int id)
        {
            var league = await dbContext.Leagues.FirstOrDefaultAsync(l => l.ID == id);
            if(league == null)
            {
                return GetCodeResult(HttpStatusCode.NotFound, $"No League with ID '{id}' found.");
            }

            var dto = new LeagueDto(league);
            return CodeResult(HttpStatusCode.OK, dto);
        }

        [HttpPost("")]
        [SwaggerResponse(HttpStatusCode.Created, typeof(LeagueDto))]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(string), "Can't find game.")]
        [SwaggerResponse(HttpStatusCode.Conflict, typeof(string), "League name taken.")]
        public async Task<IActionResult> Create(CreateRequest request)
        {
            try
            {
                var league = await leagueService.Create(request.Name, request.GameID, request.AdminID);
                var dto = new LeagueDto(league);
                return CodeResult(HttpStatusCode.Created, dto);
            }
            catch(Exception exception)
            {
                return GetExceptionResult(exception, request);
            }
        }

        [HttpPost("join")]
        [SwaggerResponse(HttpStatusCode.Created, typeof(LeagueUserDto))]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(string), "Can't find league.")]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(string), "Can't find user.")]
        public async Task<IActionResult> Join(JoinRequest request)
        {
            try
            {
                var leagueUser = await leagueService.Join(request.LeagueID, request.UserID);
                var dto = new LeagueUserDto(leagueUser, cdnService);
                return GetCodeResult(HttpStatusCode.Created, dto, "User joined league.");
            }
            catch(Exception exception)
            {
                return GetExceptionResult(exception, request);
            }
        }

        [HttpGet("{id:int}/seasons")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(IEnumerable<SeasonDto>))]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(string), "Can't find league.")]
        public async Task<IActionResult> GetSeasons(int leagueID)
        {
            var league = await dbContext.Leagues.Include(l => l.Seasons).AsNoTracking().FirstOrDefaultAsync(l => l.ID == leagueID);
            if(league == null)
            {
                return GetCodeResult(HttpStatusCode.NotFound, $"No League with ID '{leagueID}' found.");
            }

            var dtos = league.Seasons.Select(s => new SeasonDto(s));
            return CodeResult(HttpStatusCode.OK, dtos);
        }

        [HttpPost("{id:int}/update-standings")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(LeagueDto), "League power rankings have been updated.")]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(string), "Can't find league.")]
        public async Task<IActionResult> UpdateStandings(int leagueID, [FromHeader] string key)
        {
            if(key != adminKey)
            {
                return Unauthorized();
            }

            try
            {
                var league = await leagueService.UpdateStandings(leagueID);
                var dto = new LeagueDto(league);
                return Ok(dto);
            }
            catch(Exception exception)
            {
                return GetExceptionResult(exception, new {leagueID});
            }
        }

        [HttpGet("{id:int}/sets")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(SetDto[]))]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(string), "Can't find league.")]
        public async Task<IActionResult> Sets(int id, DateTime dueDate)
        {
            var league = await dbContext.Leagues.FirstOrDefaultAsync(l => l.ID == id);
            if(league == null)
            {
                return NotFound(new {leagueID = id});
            }

            var sets = await dbContext.Sets
                .Include(s => s.Player1).AsNoTracking()
                .Include(s => s.Player2).AsNoTracking()
                .Include(s => s.League).AsNoTracking()
                .Where(s => s.LeagueID == id && !s.IsComplete && s.DueDate <= dueDate)
                .Select(s => SetDto.Create(s, league.GameID))
                .ToArrayAsync();

            return Ok(sets);
        }

        [HttpGet("{id:int}/members")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(LeagueUserDto[]))]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(string))]
        public async Task<IActionResult> GetMembers(int id)
        {
            var league = await dbContext.Leagues
                .Include(l => l.Members).ThenInclude(lu => lu.User).AsNoTracking()
                .FirstOrDefaultAsync(l => l.ID == id);
            if(league == null)
            {
                return GetCodeResult(HttpStatusCode.NotFound, $"Could not find League with ID '{id}'.");
            }

            var response = league.Members.Select(lu => new LeagueUserDto(lu, cdnService));
            return CodeResult(HttpStatusCode.OK, response);
        }

        [HttpGet("members/{userID:int}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(LeagueUserDto))]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(string))]
        public async Task<IActionResult> GetMember(int userID)
        {
            var leagueUser = await dbContext.LeagueUsers.Include(lu => lu.User).AsNoTracking().FirstOrDefaultAsync(lu => lu.ID == userID);
            if(leagueUser == null)
            {
                return GetCodeResult(HttpStatusCode.NotFound, $"Could not find League User with ID '{userID}'.");
            }

            var response = new LeagueUserDto(leagueUser, cdnService);
            return CodeResult(HttpStatusCode.OK, response);
        }

        [HttpPost("leave/{leagueUserID:int}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(LeagueUserDto))]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(string), "Can't find user.")]
        public async Task<IActionResult> Leave(int leagueUserID)
        {
            try
            {
                var leagueUser = await leagueService.Leave(leagueUserID);
                var dto = new LeagueUserDto(leagueUser, cdnService);
                return GetCodeResult(HttpStatusCode.OK, dto, "User left league.");
            }
            catch(Exception exception)
            {
                return GetExceptionResult(exception, new {leagueUserID});
            }
        }

        [HttpGet("recent-characters/{leagueUserID:int}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(CharacterDto[]))]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(string), "Can't find league user.")]
        public async Task<IActionResult> GetRecentCharacters(int leagueUserID, int characterCount)
        {
            try
            {
                var characters = await leagueService.GetUsersRecentCharactersAsync(leagueUserID, characterCount);
                var dtos = characters.Select(c => CharacterDto.Create(c, cdnService));
                return Ok(dtos);
            }
            catch(Exception exception)
            {
                return GetExceptionResult(exception, new {leagueUserID, characterCount});
            }
        }

        [HttpGet("sets/{leagueUserID:int}")]
        public async Task<IActionResult> GetSets(int leagueUserID)
        {
            var leagueUser = await dbContext.LeagueUsers
                .Include(lu => lu.League).AsNoTracking()
                .FirstOrDefaultAsync(lu => lu.ID == leagueUserID);

            var sets = await dbContext.Sets
                .Include(s => s.League).AsNoTracking()
                .Include(s => s.Matches).ThenInclude(m => m.MatchCharacters).AsNoTracking()
                .Include(s => s.Player1).AsNoTracking()
                .Include(s => s.Player2).AsNoTracking()
                .Where(s => s.IsComplete && s.IsPlaying(leagueUserID))
                .ToArrayAsync();

            var dtos = sets.Select(s => SetDto.Create(s, leagueUser.League.GameID)).ToArray();
            return CodeResult(HttpStatusCode.OK, dtos);
        }
    }
}