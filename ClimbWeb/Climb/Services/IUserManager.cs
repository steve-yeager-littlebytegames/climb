using System.Security.Claims;
using System.Threading.Tasks;
using Climb.Data;
using Microsoft.AspNetCore.Identity;

namespace Climb.Services
{
    public interface IUserManager
    {
        Task<ApplicationUser> GetUserAsync(ClaimsPrincipal principal);
        Task<string> GeneratePasswordResetTokenAsync(ApplicationUser user);
        Task<IdentityResult> CreateAsync(ApplicationUser user, string password);
        Task<string> GenerateEmailConfirmationTokenAsync(ApplicationUser user);
        Task<IdentityResult> ResetPasswordAsync(ApplicationUser user, string token, string newPassword);
    }
}