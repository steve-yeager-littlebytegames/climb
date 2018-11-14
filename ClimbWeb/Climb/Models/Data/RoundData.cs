using System.Collections.Generic;

namespace Climb.Models
{
    public class RoundData
    {
        public int Index { get; }
        public string Name { get; set; }
        public List<GameData> Games { get; } = new List<GameData>();

        public RoundData(int index)
        {
            Index = index;
        }
    }
}