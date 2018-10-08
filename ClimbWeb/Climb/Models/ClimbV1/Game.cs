using System.Collections.Generic;
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
#pragma warning disable 169

namespace ClimbV1.Models
{
    public class Game
    {
        private class CharacterPercentages
        {
            public decimal matches;
            public decimal wins;
        }

        public int ID { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string BannerPicUrl { get; set; }
        public int CharactersPerMatch { get; set; }
        public bool RequireStage { get; set; }
        public string SetRules { get; set; }
        public int MaxMatchPoints { get; set; }
        public HashSet<Character> Characters { get; set; }
        public HashSet<Stage> Stages { get; set; }
        public HashSet<League> Leagues { get; set; }
    }
}