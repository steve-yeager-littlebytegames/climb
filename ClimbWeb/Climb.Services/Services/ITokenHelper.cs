using System;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;

namespace Climb.Services
{
    public interface ITokenHelper
    {
        string CreateUserToken(SecurityKey securityKey, DateTime expires, string email);
        Task<string> GetAuthorizedUserID(string token);
    }
}