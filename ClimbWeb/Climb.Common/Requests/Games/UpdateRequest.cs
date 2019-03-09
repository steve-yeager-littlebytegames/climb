using System.ComponentModel.DataAnnotations;
using Climb.Attributes.Validation;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Climb.Requests.Games
{
    public class UpdateRequest
    {
        [HiddenInput]
        public int? GameID { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int CharactersPerMatch { get; set; }
        [Required]
        public int MaxMatchPoints { get; set; }
        public string ScoreName { get; set; }
        public string MatchName { get; set; }
        [FileSize(ClimbImageRules.GameLogoMaxSize)]
        public IFormFile LogoImage { get; set; }
        [FileSize(ClimbImageRules.GameBannerMaxSize)]
        public IFormFile BannerImage { get; set; }

        [UsedImplicitly]
        public UpdateRequest()
        {
        }

        public UpdateRequest(string name, int characterCount, int maxPoints, IFormFile logo, IFormFile banner)
        {
            Name = name;
            CharactersPerMatch = characterCount;
            MaxMatchPoints = maxPoints;
            LogoImage = logo;
            BannerImage = banner;
        }
    }
}