using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Climb.Models
{
    public class Character
    {
        public int ID { get; set; }
        [Required]
        public string Name { get; set; }
        public int GameID { get; set; }
        public string ImageKey { get; set; }

        [JsonIgnore]
        public Game Game { get; set; }
    }
}