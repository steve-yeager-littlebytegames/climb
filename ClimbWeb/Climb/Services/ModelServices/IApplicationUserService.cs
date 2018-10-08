using System.Threading.Tasks;
using Climb.Data;
using Climb.Requests.Account;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Climb.Services.ModelServices
{
    public interface IApplicationUserService
    {
        Task<ApplicationUser> Register(RegisterRequest request, IUrlHelper urlHelper, string requestScheme);
        Task<string> LogIn(LoginRequest request);
        Task<string> UploadProfilePic(string userID, IFormFile image);
        Task UpdateSettings(string userID, string username, string name, IFormFile profilePic);
    }
}