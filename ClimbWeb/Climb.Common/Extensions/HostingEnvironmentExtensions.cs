using Microsoft.AspNetCore.Hosting;

namespace Climb.Extensions
{
    public static class HostingEnvironmentExtensions
    {
        public static bool IsSiteAdmin(this IHostingEnvironment environment)
        {
            return environment.EnvironmentName == "SiteAdmin";
        }
    }
}