using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace Climb.Jobs
{
    public static class UpdateLeagueRanks
    {
        [FunctionName("UpdateLeagueRanks")]
        public static async Task Run([TimerTrigger("0 0 10 * * Mon")] TimerInfo myTimer, TraceWriter log)
        {
            string key = GetVariable("Key");
            var climbUri =GetVariable("ClimbUri");

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri($"{climbUri}/admin/update-all-leagues");
                client.DefaultRequestHeaders.Add("key", key);

                var response = await client.PostAsync("", null);
                log.Info($"Update league ranks {(response.IsSuccessStatusCode ? "SUCCESS" : "FAIL")}");
            }

            log.Info($"C# Timer trigger function executed at: {DateTime.Now} with key={key}");
        }

        private static string GetVariable(string name) => Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
    }
}