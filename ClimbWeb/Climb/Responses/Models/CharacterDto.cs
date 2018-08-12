using Climb.Models;
using Climb.Services;

namespace Climb.Responses.Models
{
    public class CharacterDto
    {
        public int ID { get; }
        public string Name { get; }
        public string Picture { get; }

        private CharacterDto(int id, string name, string picture)
        {
            ID = id;
            Name = name;
            Picture = picture;
        }

        public static CharacterDto Create(Character character, ICdnService cdnService)
        {
            var picture = cdnService.GetImageUrl(character.ImageKey, ClimbImageRules.CharacterPic);
            return new CharacterDto(character.ID, character.Name, picture);
        }
    }
}