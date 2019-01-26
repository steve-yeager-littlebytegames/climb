using System.ComponentModel.DataAnnotations;
using Climb.Models;
using Climb.Services;

namespace Climb.Responses
{
    public class UserDto
    {
        [Required]
        public string ID { get; }
        [Required]
        public string Username { get; }
        [Required]
        public string Email { get; }
        [Required]
        public string ProfilePic { get; }

        private UserDto(string id, string username, string email, string profilePic)
        {
            ID = id;
            Username = username;
            Email = email;
            ProfilePic = profilePic;
        }

        public static UserDto Create(ApplicationUser applicationUser, ICdnService cdnService)
        {
            var profilePicUrl = cdnService.GetImageUrl(applicationUser.ProfilePicKey, ClimbImageRules.ProfilePic);
            return new UserDto(applicationUser.Id, applicationUser.UserName, applicationUser.Email, profilePicUrl);
        }
    }
}