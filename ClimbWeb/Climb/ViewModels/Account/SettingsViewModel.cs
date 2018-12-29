using Climb.Models;
using Climb.Requests.Account;
using Climb.Services;

namespace Climb.ViewModels.Account
{
    public class SettingsViewModel : RequestViewModel<UpdateSettingsRequest>
    {
        public string ProfilePic { get; }

        private SettingsViewModel(ApplicationUser user, string profilePic)
            : base(user)
        {
            ProfilePic = profilePic;
        }

        public static SettingsViewModel Create(ApplicationUser user, ICdnService cdnService)
        {
            var profilePic = cdnService.GetImageUrl(user.ProfilePicKey, ClimbImageRules.ProfilePic);
            return new SettingsViewModel(user, profilePic);
        }
    }
}