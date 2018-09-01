using Climb.Models;
using Climb.Services;

namespace Climb.ViewModels.Seasons
{
    public class SeasonParticipantViewModel
    {
        public SeasonLeagueUser Participant { get; }
        public string ProfilePic { get; }

        private SeasonParticipantViewModel(SeasonLeagueUser participant, string profilePic)
        {
            Participant = participant;
            ProfilePic = profilePic;
        }

        public static SeasonParticipantViewModel Create(SeasonLeagueUser participant, ICdnService cdnService)
        {
            var user = participant.LeagueUser.User;
            var profilePic = cdnService.GetUserProfilePicUrl(user.Id, user.ProfilePicKey, ClimbImageRules.ProfilePic);
            return new SeasonParticipantViewModel(participant, profilePic);
        }
    }
}