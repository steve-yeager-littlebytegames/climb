using System.ComponentModel.DataAnnotations;
using System.Linq;
using Climb.Models;
using Climb.Services;
using JetBrains.Annotations;

namespace Climb.Responses.Models
{
    public class GameDto
    {
        public int ID { get; }
        [Required, UsedImplicitly]
        public string Name { get; }
        [Required, UsedImplicitly]
        public CharacterDto[] Characters { get; }
        [Required, UsedImplicitly]
        public StageDto[] Stages { get; }
        [UsedImplicitly]
        public int CharactersPerMatch { get; }
        [Required, UsedImplicitly]
        public string ScoreName { get; }
        [Required, UsedImplicitly]
        public string MatchName { get; }
        [UsedImplicitly]
        public int MaxMatchPoints { get; }

        private GameDto(Game game, CharacterDto[] characters, StageDto[] stages)
        {
            ID = game.ID;
            Name = game.Name;
            ScoreName = game.ScoreName;
            MatchName = game.MatchName;
            CharactersPerMatch = game.CharactersPerMatch;
            MaxMatchPoints = game.MaxMatchPoints;
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