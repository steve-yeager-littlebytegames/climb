using System.Collections.Generic;

namespace Climb.Services
{
    public partial class BracketGenerator
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
}