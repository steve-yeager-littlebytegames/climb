using System.ComponentModel.DataAnnotations;

namespace Climb.Requests.Games
{
    public class AddStageRequest
    {
        [Required]
        public int GameID { get; set; }
        [Required]
        public string Name { get; set; }
        public int? StageID { get; set; }
    }
}