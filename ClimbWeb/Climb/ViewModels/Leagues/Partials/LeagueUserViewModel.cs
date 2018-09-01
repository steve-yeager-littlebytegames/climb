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
        public int EmptyCharacterCount { get; }
        public string RankWhiteSpace { get; }

        private LeagueUserViewModel(LeagueUser leagueUser, string title, string titleLink, string picture, IEnumerable<string> characters, int emptyCharacterCount, string rankWhiteSpace)
        {
            LeagueUser = leagueUser;
            Picture = picture;
            Characters = characters;
            EmptyCharacterCount = emptyCharacterCount;
            RankWhiteSpace = rankWhiteSpace;
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

            const int maxCharacterCount = 3;
            var characters = await leagueService.GetUsersRecentCharactersAsync(leagueUser.ID, maxCharacterCount);
            var characterUrls = characters.Select(c => cdnService.GetImageUrl(c.ImageKey, ClimbImageRules.CharacterPic));
            var emptyCharacterCount = maxCharacterCount - characters.Count;

            var rank = leagueUser.Rank.ToString();
            var rankWhiteSpace = "";
            for (int i = 0; i < 3 - rank.Length; i++)
            {
                rankWhiteSpace += "0";
            }

            return new LeagueUserViewModel(leagueUser, title, titleLink, picture, characterUrls, emptyCharacterCount, rankWhiteSpace);
        }
    }
}