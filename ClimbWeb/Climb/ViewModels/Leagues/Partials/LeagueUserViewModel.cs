using Climb.Models;
using Climb.Services;
using Microsoft.AspNetCore.Mvc;

namespace Climb.ViewModels.Leagues
{
    public class LeagueUserViewModel
    {
        public LeagueUser LeagueUser { get; }
        public string Title { get; }
        public string TitleLink { get; }
        public string Picture { get; }

        private LeagueUserViewModel(LeagueUser leagueUser, string title, string titleLink, string picture)
        {
            LeagueUser = leagueUser;
            Picture = picture;
            TitleLink = titleLink;
            Title = title;
        }

        public static LeagueUserViewModel Create(LeagueUser leagueUser, ICdnService cdnService, bool showUser, IUrlHelper urlHelper)
        {
            string title;
            string titleLink;
            string picture;

            if(showUser)
            {
                title = leagueUser.DisplayName;
                titleLink = urlHelper.Action("Home", "User", new {leagueUser.UserID});
                picture = leagueUser.User.GetProfilePicUrl(cdnService);
            }
            else
            {
                title = leagueUser.League.Name;
                titleLink = urlHelper.Action("Home", "League", new {leagueUser.LeagueID});
                picture = cdnService.GetImageUrl(leagueUser.League.Game.LogoImageKey, ClimbImageRules.GameLogo);
            }

            return new LeagueUserViewModel(leagueUser, title, titleLink, picture);
        }
    }
}