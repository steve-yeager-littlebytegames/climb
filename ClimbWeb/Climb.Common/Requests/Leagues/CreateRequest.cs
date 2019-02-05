using System.ComponentModel.DataAnnotations;

namespace Climb.Requests.Leagues
{
    public class CreateRequest
    {
        [Required]
        public int GameID { get; set; }
        [Required]
        public string Name { get; set; }
        public string AdminID { get; set; }
    }
}