﻿using System.ComponentModel.DataAnnotations;

namespace Climb.Requests.Games
{
    public class CreateRequest
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public int CharactersPerMatch { get; set; }
        [Required]
        public int MaxMatchPoints { get; set; }

        public CreateRequest()
        {
        }

        public CreateRequest(string name, int characterCount, int maxPoints)
        {
            Name = name;
            CharactersPerMatch = characterCount;
            MaxMatchPoints = maxPoints;
        }
    }
}