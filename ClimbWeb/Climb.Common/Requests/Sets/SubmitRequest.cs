using System.ComponentModel.DataAnnotations;

namespace Climb.Requests.Sets
{
    public class SubmitRequest
    {
        [Required]
        public int SetID { get; set; }
        [Required]
        public MatchForm[] Matches { get; set; }
    }
}