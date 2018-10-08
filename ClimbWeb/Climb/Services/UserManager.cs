using System.Security.Claims;
using System.Threading.Tasks;
using Climb.Data;
using Microsoft.AspNetCore.Identity;

namespace Climb.Services
{
    public class UserManager : IUserManager
    {
        private readonly UserManager<ApplicationUser> userManager;

        public UserManager(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }

        public Task<ApplicationUser> GetUserAsync(ClaimsPrincipal principal)
        {
            return userManager.GetUserAsync(principal);
        }

        public Task<string> GeneratePasswordResetTokenAsync(ApplicationUser user)
        {
            return userManager.GeneratePasswordResetTokenAsync(user);
        }

        public Task<IdentityResult> CreateAsync(ApplicationUser user, string password)
        {
            return userManager.CreateAsync(user, password);
        }

        public Task<string> GenerateEmailConfirmationTokenAsync(ApplicationUser user)
        {
            return userManager.GenerateEmailConfirmationTokenAsync(user);
        }

        public Task<IdentityResult> ResetPasswordAsync(ApplicationUser user, string token, string newPassword)
        {
            return userManager.ResetPasswordAsync(user, token, newPassword);
        }
    }
}