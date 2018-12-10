using System;
using System.Net;
using System.Threading.Tasks;
using Climb.Attributes;
using Climb.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
<<<<<<< HEAD
<<<<<<< master
<<<<<<< master
using Newtonsoft.Json;
=======
>>>>>>> Setup class and start API tests
=======
using Newtonsoft.Json;
>>>>>>> Send data to WPF app
=======
using Newtonsoft.Json;
>>>>>>> data-analyzer

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
<<<<<<< HEAD
<<<<<<< master
<<<<<<< master
        [SwaggerResponse(HttpStatusCode.OK, typeof(string))]
=======
        [SwaggerResponse(HttpStatusCode.OK, typeof(AnalyzerDataCollection))]
>>>>>>> Setup class and start API tests
=======
        [SwaggerResponse(HttpStatusCode.OK, typeof(string))]
>>>>>>> Send data to WPF app
=======
        [SwaggerResponse(HttpStatusCode.OK, typeof(string))]
>>>>>>> data-analyzer
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
<<<<<<< HEAD
<<<<<<< master
<<<<<<< master
                return CodeResult(HttpStatusCode.OK, JsonConvert.SerializeObject(result));
=======
                return CodeResult(HttpStatusCode.OK, result);
>>>>>>> Setup class and start API tests
=======
                return CodeResult(HttpStatusCode.OK, JsonConvert.SerializeObject(result));
>>>>>>> Send data to WPF app
=======
                return CodeResult(HttpStatusCode.OK, JsonConvert.SerializeObject(result));
>>>>>>> data-analyzer
            }
            catch (Exception exception)
            {
                return GetExceptionResult(exception, new {player1ID, player2ID});
            }
        }
    }
}