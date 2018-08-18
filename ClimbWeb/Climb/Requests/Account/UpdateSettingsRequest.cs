using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Climb.Requests.Account
{
    public class UpdateSettingsRequest
    {
        // TODO: Encapsulate so it can be used when registering too.
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        public string Username { get; set; }
        public string Name { get; set; }
        public IFormFile ProfilePic { get; set; }
    }
}