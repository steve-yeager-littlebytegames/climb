using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Climb.Requests.Games
{
    public class UpdateRequest
    {
        public int? GameID { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int CharactersPerMatch { get; set; }
        [Required]
        public int MaxMatchPoints { get; set; }
        [Required]
        public bool HasStages { get; set; }
        [Required]
        public IFormFile LogoImage { get; set; }

        public UpdateRequest()
        {
        }

        public UpdateRequest(string name, int characterCount, int maxPoints, bool hasStages, IFormFile logo)
        {
            Name = name;
            CharactersPerMatch = characterCount;
            MaxMatchPoints = maxPoints;
            HasStages = hasStages;
            LogoImage = logo;
        }
    }
}