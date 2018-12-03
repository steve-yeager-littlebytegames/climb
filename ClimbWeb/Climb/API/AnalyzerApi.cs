using System;
using System.Net;
using System.Threading.Tasks;
using Climb.Attributes;
using Climb.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Climb.API
{
    [Route("api/v1/data")]
    public class AnalyzerApi : BaseApi<AnalyzerApi>
    {
        private readonly IAnalyzerService analyzerService;

        public AnalyzerApi(ILogger<AnalyzerApi> logger, IAnalyzerService analyzerService)
            : base(logger)
        {
            this.analyzerService = analyzerService;
        }

        [HttpGet("analyze")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(AnalyzerDataCollection))]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(string))]
        public async Task<IActionResult> Analyze(int player1ID, int player2ID)
        {
            if(player1ID == player2ID)
            {
                return BadRequest("Requires 2 different player IDs.");
            }

            try
            {
                var result = await analyzerService.Calculate(player1ID, player2ID);
                return CodeResult(HttpStatusCode.OK, result);
            }
            catch (Exception exception)
            {
                return GetExceptionResult(exception, new {player1ID, player2ID});
            }
        }
    }
}