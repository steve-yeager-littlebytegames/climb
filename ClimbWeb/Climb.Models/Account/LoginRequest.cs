using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Climb.Requests.Account
{
    public class LoginRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        public bool RememberMe { get; set; }
        [HiddenInput]
        public string ReturnUrl { get; set; }
    }
}