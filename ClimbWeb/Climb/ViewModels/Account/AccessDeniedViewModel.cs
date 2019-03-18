using Climb.Models;

namespace Climb.ViewModels.Account
{
    public class AccessDeniedViewModel : BaseViewModel
    {
        public string ReturnUrl { get; }

        public AccessDeniedViewModel(ApplicationUser user, string returnUrl)
            : base(user)
        {
            ReturnUrl = returnUrl;
        }
    }
}