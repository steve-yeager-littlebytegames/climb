using System.ComponentModel.DataAnnotations;
using Climb.Attributes.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Climb.Requests.Games
{
    public class AddCharacterRequest
    {
        [Required]
        [HiddenInput]
        public int GameID { get; set; }
        [HiddenInput]
        public int? CharacterID { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 1)]
        public string Name { get; set; }
        [FileSize(ClimbImageRules.CharacterImageMaxSize)]
        public IFormFile Image { get; set; }
    }
}