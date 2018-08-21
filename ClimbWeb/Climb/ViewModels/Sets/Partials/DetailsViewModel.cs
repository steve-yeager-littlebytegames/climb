using Climb.Data;
using Climb.Models;
using Climb.Services;

namespace Climb.ViewModels.Sets
{
    public class DetailsViewModel
    {
        public Set Set { get; }
        public string P1ProfilePic { get; }
        public string P2ProfilePic { get; }
        public bool UserIsPlaying { get; }
        public string OpponentProfilePic { get; }
        public string SetType { get; }

        private DetailsViewModel(Set set, string p1ProfilePic, string p2ProfilePic, bool userIsPlaying, string opponentProfilePic)
        {
            Set = set;
            P1ProfilePic = p1ProfilePic;
            P2ProfilePic = p2ProfilePic;
            UserIsPlaying = userIsPlaying;
            OpponentProfilePic = opponentProfilePic;
            SetType = set.Season != null ? "Season" : "Challenge";
        }

        public static DetailsViewModel Create(ApplicationUser viewingUser, Set set, ICdnService cdnService)
        {
            var player1 = set.Player1;
            var player2 = set.Player2;

            var p1ProfilePic = cdnService.GetUserProfilePicUrl(player1.User.Id, player1.User.ProfilePicKey, ClimbImageRules.ProfilePic);
            var p2ProfilePic = cdnService.GetUserProfilePicUrl(player2.User.Id, player2.User.ProfilePicKey, ClimbImageRules.ProfilePic);
            var isPlaying = viewingUser != null && (player1.UserID == viewingUser.Id || player2.UserID == viewingUser.Id);
            var opponentProfilePic = player1.UserID == viewingUser?.Id ? p2ProfilePic : p1ProfilePic;

            return new DetailsViewModel(set, p1ProfilePic, p2ProfilePic, isPlaying, opponentProfilePic);
        }
    }
}