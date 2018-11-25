using System.Collections.Generic;
using System.Linq;
using Climb.Data;
using Climb.Extensions;
using Climb.Models;
using Climb.Services;
using Microsoft.Extensions.Configuration;

namespace Climb.ViewModels
{
    public class BaseViewModel
    {
        public ApplicationUser User { get; }
        public IReadOnlyList<League> UserActiveLeagues { get; }
        public IReadOnlyList<Season> UserSeasons { get; }
        public bool IsAdminMode { get; }

        public bool IsLoggedIn => User != null;

        public BaseViewModel(ApplicationUser user, IConfiguration configuration)
        {
            User = user;
            IsAdminMode = configuration.IsDevMode(DevModes.Admin);

            if(user == null)
            {
                UserActiveLeagues = new League[0];
                UserSeasons = new Season[0];
            }
            else
            {
                var leagues = user.LeagueUsers
                    .Select(lu => lu.League)
                    .OrderBy(l => l.Name)
                    .ToArray();
                UserActiveLeagues = leagues;

                UserSeasons = user.Seasons
                    .Where(slu => !slu.Season.IsComplete)
                    .Select(slu => slu.Season).ToArray();
            }
        }

        public string GetProfilePic(ICdnService cdnService)
        {
            return cdnService.GetUserProfilePicUrl(User.Id, User.ProfilePicKey, ClimbImageRules.ProfilePic);
        }

        private bool IsSubPageActive<T>() where T : BaseViewModel => this is T;
        public string SubPageNavbarClass<T>() where T : BaseViewModel => IsSubPageActive<T>() ? "active" : "";
    }
}