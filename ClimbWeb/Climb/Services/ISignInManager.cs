using System.Threading.Tasks;
using Climb.Data;
using Microsoft.AspNetCore.Identity;

namespace Climb.Services
{
    public interface ISignInManager
    {
        Task SignInAsync(ApplicationUser user, bool isPersistent);
        Task<SignInResult> PasswordSignInAsync(string userName, string password, bool isPersistent, bool lockoutOnFailure);
        Task SignOutAsync();
    }
}