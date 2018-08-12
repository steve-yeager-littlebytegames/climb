using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Climb.Attributes;
using Climb.Data;
using Climb.Models;
using Climb.Responses.Models;
using Climb.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Climb.API
{
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

        [HttpGet("/api/v1/games/{gameID:int}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(GameDto))]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(string))]
        public async Task<IActionResult> Get(int gameID)
        {
            var game = await dbContext.Games
                .Include(g => g.Characters).AsNoTracking()
                .Include(g => g.Stages).AsNoTracking()
                .FirstOrDefaultAsync(g => g.ID == gameID);
            if(game == null)
            {
                return CodeResultAndLog(HttpStatusCode.NotFound, $"Could not find Game with ID '{gameID}'.");
            }

            var dto = GameDto.Create(game, cdnService);
            return CodeResult(HttpStatusCode.OK, dto);
        }

        [HttpGet("/api/v1/games")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(List<Game>))]
        public async Task<IActionResult> ListAll()
        {
            var games = await dbContext.Games.ToListAsync();

            return CodeResult(HttpStatusCode.OK, games);
        }
    }
}