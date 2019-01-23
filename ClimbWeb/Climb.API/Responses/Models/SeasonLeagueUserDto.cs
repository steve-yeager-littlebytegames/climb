using Climb.Models;
using Climb.Services;

namespace Climb.Responses.Models
{
    public class SeasonLeagueUserDto
    {
        public int ID { get; }
        public int SeasonID { get; }
        public int LeagueUserID { get; }
        public string UserID { get; }
        public int Standing { get; }
        public int Points { get; }
        public int TieBreakerPoints { get; }
        public bool HasLeft { get; }
        public string ProfilePic { get; }

        public SeasonLeagueUserDto(SeasonLeagueUser seasonLeagueUser, ICdnService cdnService)
        {
            ID = seasonLeagueUser.ID;
            SeasonID = seasonLeagueUser.SeasonID;
            LeagueUserID = seasonLeagueUser.LeagueUserID;
            UserID = seasonLeagueUser.UserID;
            Standing = seasonLeagueUser.Standing;
            Points = seasonLeagueUser.Points;
            TieBreakerPoints = seasonLeagueUser.TieBreakerPoints;
            HasLeft = seasonLeagueUser.HasLeft;

            ProfilePic = cdnService.GetUserProfilePicUrl(UserID, seasonLeagueUser.User?.ProfilePicKey, ClimbImageRules.ProfilePic);
        }
    }
}