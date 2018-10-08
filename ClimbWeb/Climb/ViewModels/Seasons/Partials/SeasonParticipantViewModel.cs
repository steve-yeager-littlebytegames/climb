using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Climb.Models;
using Climb.Services;
using Climb.Services.ModelServices;

namespace Climb.ViewModels.Seasons
{
    public class SeasonParticipantViewModel
    {
        public SeasonLeagueUser Participant { get; }
        public string ProfilePic { get; }
        public IEnumerable<string> Characters { get; }
        public int EmptyCharacterCount { get; }

        private SeasonParticipantViewModel(SeasonLeagueUser participant, string profilePic, IEnumerable<string> characters, int emptyCharacterCount)
        {
            Participant = participant;
            ProfilePic = profilePic;
            Characters = characters;
            EmptyCharacterCount = emptyCharacterCount;
        }

        public static async Task<SeasonParticipantViewModel> Create(SeasonLeagueUser participant, ICdnService cdnService, ILeagueService leagueService)
        {
            var user = participant.LeagueUser.User;
            var profilePic = cdnService.GetUserProfilePicUrl(user.Id, user.ProfilePicKey, ClimbImageRules.ProfilePic);

            const int maxCharacterCount = 3;
            var characters = await leagueService.GetUsersRecentCharactersAsync(participant.LeagueUserID, maxCharacterCount);
            var characterUrls = characters.Select(c => cdnService.GetImageUrl(c.ImageKey, ClimbImageRules.CharacterPic));
            var emptyCharacterCount = maxCharacterCount - characters.Count;

            return new SeasonParticipantViewModel(participant, profilePic, characterUrls, emptyCharacterCount);
        }
    }
}