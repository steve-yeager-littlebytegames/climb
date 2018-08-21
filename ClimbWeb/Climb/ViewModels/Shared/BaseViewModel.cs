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
        public IReadOnlyList<Season> UserActiveSeasons { get; }

        public bool IsLoggedIn => User != null;

        public BaseViewModel(ApplicationUser user)
        {
            User = user;

            if(user == null)
            {
                UserActiveLeagues = new League[0];
                UserActiveSeasons = new Season[0];
            }
            else
            {
                var leagues = user.LeagueUsers
                    .Select(lu => lu.League)
                    .OrderBy(l => l.Name)
                    .ToArray();
                UserActiveLeagues = leagues;

                UserActiveSeasons = user.LeagueUsers
                    .Select(lu => lu.Seasons.FirstOrDefault(slu => slu.Season.IsActive))
                    .Where(slu => slu != null)
                    .Select(slu => slu.Season).ToArray();
            }
        }

        public string GetProfilePic(ICdnService cdnService)
        {
            return cdnService.GetUserProfilePicUrl(User.Id, User.ProfilePicKey, ClimbImageRules.ProfilePic);
        }
    }
}