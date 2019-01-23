using Microsoft.AspNetCore.Mvc;

namespace Climb.Extensions
{
    public static class ActionResultExtensions
    {
        public static T GetObject<T>(this IActionResult result)
        {
            return (T)((ObjectResult)result).Value;
        }
    }
}