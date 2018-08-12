using System.Linq;
using Climb.Models;
using Climb.Services;

namespace Climb.Responses.Models
{
    public class GameDto
    {
        public int ID { get; }
        public string Name { get; }
        public CharacterDto[] Characters { get; }
        public StageDto[] Stages { get; }
        public int CharactersPerMatch { get; }
        public bool HasStages { get; }

        private GameDto(Game game, CharacterDto[] characters, StageDto[] stages)
        {
            ID = game.ID;
            Name = game.Name;
            CharactersPerMatch = game.CharactersPerMatch;
            HasStages = game.HasStages;
            Characters = characters;
            Stages = stages;
        }

        public static GameDto Create(Game game, ICdnService cdnService)
        {
            var characters = game.Characters.Select(c => CharacterDto.Create(c, cdnService)).ToArray();
            var stages = game.Stages.Select(s => new StageDto(s.ID, s.Name)).ToArray();

            return new GameDto(game, characters, stages);
        }
    }
}