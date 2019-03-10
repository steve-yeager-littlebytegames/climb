using Climb.Requests.Account;

namespace Climb.ViewModels.Account
{
    public class LogInViewModel : RequestViewModel<LoginRequest>
    {
        public string ReturnUrl { get; }

        public LogInViewModel(string returnUrl)
            : base(null)
        {
            ReturnUrl = returnUrl;
        }
    }
}