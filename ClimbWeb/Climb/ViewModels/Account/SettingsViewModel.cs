using Climb.Models;
using Climb.Requests.Account;
using Climb.Services;
using Microsoft.Extensions.Configuration;

namespace Climb.ViewModels.Account
{
    public class SettingsViewModel : RequestViewModel<UpdateSettingsRequest>
    {
        public string ProfilePic { get; }

        private SettingsViewModel(ApplicationUser user, string profilePic, IConfiguration configuration)
            : base(user, configuration)
        {
            ProfilePic = profilePic;
        }

        public static SettingsViewModel Create(ApplicationUser user, ICdnService cdnService, IConfiguration configuration)
        {
            var profilePic = cdnService.GetImageUrl(user.ProfilePicKey, ClimbImageRules.ProfilePic);
            return new SettingsViewModel(user, profilePic, configuration);
        }
    }
}