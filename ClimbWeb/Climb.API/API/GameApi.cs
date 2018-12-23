using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Climb.Attributes;
using Climb.Data;
using Climb.Responses.Models;
using Climb.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Climb.API
{
    [Route("api/v1/games")]
    public class GameApi : BaseApi<GameApi>
    {
        private readonly ApplicationDbContext dbContext;
        private readonly ICdnService cdnService;

        public GameApi(ILogger<GameApi> logger, ApplicationDbContext dbContext, ICdnService cdnService)
            : base(logger)
        {
            this.dbContext = dbContext;
            this.cdnService = cdnService;
        }

        [HttpGet("{id:int}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(GameDto))]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(string))]
        public async Task<IActionResult> Get(int id)
        {
            var game = await dbContext.Games
                .Include(g => g.Characters).AsNoTracking()
                .Include(g => g.Stages).AsNoTracking()
                .FirstOrDefaultAsync(g => g.ID == id);
            if(game == null)
            {
                return GetCodeResult(HttpStatusCode.NotFound, $"Could not find Game with ID '{id}'.");
            }

            var dto = GameDto.Create(game, cdnService);
            return CodeResult(HttpStatusCode.OK, dto);
        }

        [HttpGet("")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(IEnumerable<GameDto>))]
        public async Task<IActionResult> ListAll()
        {
            var games = await dbContext.Games.ToListAsync();
            var dtos = games.Select(g => GameDto.Create(g, cdnService));
            return CodeResult(HttpStatusCode.OK, dtos);
        }
    }
}