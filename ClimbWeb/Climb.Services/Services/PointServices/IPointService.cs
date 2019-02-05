namespace Climb.Services
{
    public interface IPointService
    {
        (int p1Points, int p2Points) CalculatePointDeltas(int p1Points, int p2Points, bool p1Won, int kFactor = 32);
    }
}