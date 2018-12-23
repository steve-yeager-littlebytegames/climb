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
    }
}