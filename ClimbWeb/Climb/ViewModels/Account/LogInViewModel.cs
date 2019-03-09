namespace Climb.ViewModels.Account
{
    public class LogInViewModel : BaseViewModel
    {
        public string ReturnUrl { get; }

        public LogInViewModel(string returnUrl)
            : base(null)
        {
            ReturnUrl = returnUrl;
        }
    }
}