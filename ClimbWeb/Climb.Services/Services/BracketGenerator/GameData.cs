using System.Diagnostics;

namespace Climb.Services
{
    public partial class BracketGenerator
    {
        public class GameData
        {
            public int ID { get; }
            public int? P1 { get; private set; }
            public int? P2 { get; private set; }
            public GameData NextWin { get; set; }
            public GameData NextLoss { get; set; }
            public GameData P1Game { get; set; }
            public GameData P2Game { get; set; }
            public int? P1Score { get; set; }
            public int? P2Score { get; set; }

            public GameData(int id, int? p1, int? p2)
            {
                ID = id;
                P1 = p1;
                P2 = p2;
            }

            public override string ToString() => $"{ID} W={NextWin?.ID} L={NextLoss?.ID}";

            public void AddPlayer(int? player)
            {
                Debug.Assert(P1 == null || P2 == null);

                if(P1 == null)
                {
                    P1 = player;
                }
                else
                {
                    P2 = player;
                }
            }
        }
    }
}