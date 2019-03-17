using Climb.Models;

namespace Climb.ViewModels.Site
{
    public class ErrorViewModel : BaseViewModel
    {
        public int ErrorCode { get; }
        public string ErrorDescription { get; }

        public ErrorViewModel(ApplicationUser user, int errorCode, string errorDescription)
            : base(user)
        {
            ErrorCode = errorCode;
            ErrorDescription = errorDescription;
        }
    }
}