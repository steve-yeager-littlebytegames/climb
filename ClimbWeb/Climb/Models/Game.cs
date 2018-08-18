using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Climb.Models
{
    public class Game
    {
        public int ID { get; set; }
        [Required]
        public string Name { get; set; } = "";
        public DateTime DateAdded { get; set; }
        public int CharactersPerMatch { get; set; }
        public int MaxMatchPoints { get; set; }
        public bool HasStages { get; set; }
        public string LogoImageKey { get; set; }

        [Required]
        public List<Character> Characters { get; set; }
        [Required]
        public List<Stage> Stages { get; set; }
        public List<League> Leagues { get; set; }

        public Game()
        {
        }

        public Game(string name, int charactersPerMatch, int maxMatchPoints, bool hasStages)
        {
            Name = name;
            DateAdded = DateTime.Today;
            CharactersPerMatch = charactersPerMatch;
            MaxMatchPoints = maxMatchPoints;
            HasStages = hasStages;
        }
    }
}