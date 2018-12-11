using System.Collections.Generic;
using System.Linq;
using Climb.Data;
using Climb.Models;
using Climb.Services;

namespace Climb.ViewModels
{
    public class BaseViewModel
    {
        public ApplicationUser User { get; }
        public IReadOnlyList<League> UserActiveLeagues { get; }
        public IReadOnlyList<Season> UserSeasons { get; }

        public bool IsLoggedIn => User != null;

        public BaseViewModel(ApplicationUser user)
        {
            User = user;

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
    }
}