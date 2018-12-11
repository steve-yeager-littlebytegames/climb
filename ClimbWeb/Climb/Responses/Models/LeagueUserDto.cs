using System.ComponentModel.DataAnnotations;
using Climb.Models;
using Climb.Services;

namespace Climb.Responses.Models
{
    public class LeagueUserDto
    {
        public int ID { get; }
        public int LeagueID { get; }
        [Required]
        public string UserID { get; }
        public bool HasLeft { get; }
        [Required]
        public string Username { get; }
        public int Points { get; }
        public int Rank { get; }
        public string ProfilePicture { get; }
        public RankTrends RankTrend { get; }

        public LeagueUserDto(LeagueUser leagueUser, ICdnService cdnService)
        {
            ID = leagueUser.ID;
            LeagueID = leagueUser.ID;
            UserID = leagueUser.UserID;
            HasLeft = leagueUser.HasLeft;
            Username = leagueUser.DisplayName;
            Points = leagueUser.Points;
            Rank = leagueUser.Rank;
            RankTrend = leagueUser.RankTrend;

            ProfilePicture = cdnService.GetUserProfilePicUrl(leagueUser.UserID, leagueUser.User?.ProfilePicKey, ClimbImageRules.ProfilePic);
        }
    }
}