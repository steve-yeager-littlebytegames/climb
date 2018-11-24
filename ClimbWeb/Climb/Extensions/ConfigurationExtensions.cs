using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Climb.Extensions
{
    public static class ConfigurationExtensions
    {
        public static SecurityKey GetSecurityKey(this IConfiguration configuration)
        {
            var securityToken = configuration["SecurityKey"];
            return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityToken));
        }

        public static bool IsDevMode(this IConfiguration configuration, DevModes devMode)
        {
            const string key = "DevMode";
            var definedMode = configuration.GetValue<DevModes>(key);
            return definedMode == devMode;
        }
    }
}