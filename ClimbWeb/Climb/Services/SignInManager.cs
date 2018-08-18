using System.Threading.Tasks;
using Climb.Data;
using Microsoft.AspNetCore.Identity;

namespace Climb.Services
{
    public class SignInManager : ISignInManager
    {
        private readonly SignInManager<ApplicationUser> signInManager;

        public SignInManager(SignInManager<ApplicationUser> signInManager)
        {
            this.signInManager = signInManager;
        }

        public Task SignInAsync(ApplicationUser user, bool isPersistent)
        {
            return signInManager.SignInAsync(user, isPersistent);
        }

        public Task<SignInResult> PasswordSignInAsync(string userName, string password, bool isPersistent, bool lockoutOnFailure)
        {
            return signInManager.PasswordSignInAsync(userName, password, isPersistent, lockoutOnFailure);
        }

        public Task SignOutAsync()
        {
            return signInManager.SignOutAsync();
        }
    }
}