using System.ComponentModel.DataAnnotations;

namespace Climb.Requests.Sets
{
    public class MatchForm
    {
        [Required]
        public int Player1Score { get; set; }
        [Required]
        public int Player2Score { get; set; }
        [Required]
        public int[] Player1Characters { get; set; }
        [Required]
        public int[] Player2Characters { get; set; }
        public int? StageID { get; set; }
    }
}