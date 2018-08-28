using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Climb.Models;
using Climb.Services;
using Climb.Services.ModelServices;
using Microsoft.AspNetCore.Mvc;

namespace Climb.ViewModels.Leagues
{
    public class LeagueUserViewModel
    {
        public LeagueUser LeagueUser { get; }
        public string Title { get; }
        public string TitleLink { get; }
        public string Picture { get; }
        public IEnumerable<string> Characters { get; }

        private LeagueUserViewModel(LeagueUser leagueUser, string title, string titleLink, string picture, IEnumerable<string> characters)
        {
            LeagueUser = leagueUser;
            Picture = picture;
            Characters = characters;
            TitleLink = titleLink;
            Title = title;
        }

        public static async Task<LeagueUserViewModel> Create(LeagueUser leagueUser, ICdnService cdnService, bool showUser, IUrlHelper urlHelper, ILeagueService leagueService)
        {
            string title;
            string titleLink;
            string picture;

            if(showUser)
            {
                title = leagueUser.DisplayName;
                titleLink = urlHelper.Action("Home", "User", new {leagueUser.UserID});
                picture = cdnService.GetUserProfilePicUrl(leagueUser.User.Id, leagueUser.User.ProfilePicKey, ClimbImageRules.ProfilePic);
            }
            else
            {
                title = leagueUser.League.Name;
                titleLink = urlHelper.Action("Home", "League", new {leagueUser.LeagueID});
                picture = cdnService.GetImageUrl(leagueUser.League.Game.LogoImageKey, ClimbImageRules.GameLogo);
            }

            var characters = await leagueService.GetUsersRecentCharactersAsync(leagueUser.ID, 3);
            var characterUrls = characters.Select(c => cdnService.GetImageUrl(c.ImageKey, ClimbImageRules.CharacterPic));

            return new LeagueUserViewModel(leagueUser, title, titleLink, picture, characterUrls);
        }
    }
}