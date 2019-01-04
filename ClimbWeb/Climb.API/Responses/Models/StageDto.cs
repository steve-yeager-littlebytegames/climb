using System.ComponentModel.DataAnnotations;

namespace Climb.Responses.Models
{
    public class StageDto
    {
        public int ID { get; }
        [Required]
        public string Name { get; }

        public StageDto(int id, string name)
        {
            ID = id;
            Name = name;
        }
    }
}