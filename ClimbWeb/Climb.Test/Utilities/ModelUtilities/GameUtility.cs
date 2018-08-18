using System.Collections.Generic;
using Climb.Data;
using Climb.Models;

namespace Climb.Test.Utilities
{
    public static class GameUtility
    {
        public static Game Create(ApplicationDbContext dbContext, int characterCount, int stageCount, string logoKey = null, string name = "Test Game")
        {
            var game = new Game(name, characterCount, stageCount, stageCount > 0)
            {
                Characters = new List<Character>(characterCount),
                Stages = new List<Stage>(stageCount),
                LogoImageKey = logoKey,
            };
            dbContext.Add(game);

            for(var i = 0; i < characterCount; i++)
            {
                var character = new Character {Name = $"Character {i}"};
                game.Characters.Add(character);
                dbContext.Add(character);
            }

            for(var i = 0; i < stageCount; i++)
            {
                var stage = new Stage {Name = $"Character {i}"};
                game.Stages.Add(stage);
                dbContext.Add(stage);
            }

            dbContext.SaveChanges();
            return game;
        }
    }
}