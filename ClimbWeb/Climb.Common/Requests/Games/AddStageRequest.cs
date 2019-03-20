using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Climb.Requests.Games
{
    public class AddStageRequest
    {
        [Required]
        [HiddenInput]
        public int GameID { get; set; }
        [Required]
        public string Name { get; set; }
        [HiddenInput]
        public int? StageID { get; set; }
    }
}