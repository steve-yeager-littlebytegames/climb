using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Climb.Models
{
    public class Game
    {
        public const string DefaultScoreName = "Rounds Taken";
        public const string DefaultMatchName = "Match";

        [UsedImplicitly]
        public int ID { get; set; }
        public string Name { get; set; } = "";
        [UsedImplicitly]
        public DateTime DateAdded { get; private set; }
        public int CharactersPerMatch { get; set; }
        public int MaxMatchPoints { get; set; }
        public string LogoImageKey { get; set; }
        public string ScoreName { get; set; }
        public string MatchName { get; set; }

        public List<Character> Characters { get; set; }
        public List<Stage> Stages { get; set; }
        [UsedImplicitly]
        public List<League> Leagues { get; } = new List<League>();

        public Game()
        {
        }

        public Game(string name, int charactersPerMatch, int maxMatchPoints, DateTime dateAdded)
        {
            Name = name;
            CharactersPerMatch = charactersPerMatch;
            MaxMatchPoints = maxMatchPoints;
            DateAdded = dateAdded;
        }
    }
}