using Microsoft.AspNetCore.Http;

namespace Climb.Extensions
{
    public static class HttpRequestExtensions
    {
        public static string GetReferer(this HttpRequest request)
        {
            return request.Headers["Referer"];
        }
    }
}