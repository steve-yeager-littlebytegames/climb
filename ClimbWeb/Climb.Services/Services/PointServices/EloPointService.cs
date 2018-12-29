using System;

namespace Climb.Services
{
    public class EloPointService : IPointService
    {
        public (int p1Points, int p2Points) CalculatePointDeltas(int p1Points, int p2Points, bool p1Won, int kFactor = 32)
        {
            var transformedP1 = GetTransformedScore(p1Points);
            var transformedP2 = GetTransformedScore(p2Points);

            var expectedP1 = transformedP1 / (transformedP1 + transformedP2);
            var expectedP2 = transformedP2 / (transformedP1 + transformedP2);

            var scoreP1 = p1Won ? 1 : 0;
            var scoreP2 = !p1Won ? 1 : 0;

            var eloDeltaP1 = kFactor * (scoreP1 - expectedP1);
            var eloDeltaP2 = kFactor * (scoreP2 - expectedP2);

            return ((int)Math.Round(eloDeltaP1), (int)Math.Round(eloDeltaP2));

            double GetTransformedScore(int winnerScore)
            {
                return Math.Pow(10, (double)winnerScore / 400);
            }
        }
    }
}