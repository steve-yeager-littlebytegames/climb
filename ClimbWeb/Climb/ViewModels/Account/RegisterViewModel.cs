using Climb.Data;
using Climb.Requests.Account;

namespace Climb.ViewModels.Account
{
    public class RegisterViewModel : BaseViewModel
    {
        public RegisterRequest Request { get; }

        public RegisterViewModel(ApplicationUser user, RegisterRequest request)
            : base(user)
        {
            Request = request;
        }
    }
}