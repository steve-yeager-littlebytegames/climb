namespace Climb.ViewModels.Tournaments
{
    public class GamePlayerViewModel
    {
        public int? PlayerID { get; }
        public int PlayerSeed { get; }
        public int? PreviousGameID { get; }
        public int Score { get; }
        public bool IsWinner { get; }

        public GamePlayerViewModel(int? playerID, int playerSeed, int? previousGameID, int score, bool isWinner)
        {
            PlayerID = playerID;
            PlayerSeed = playerSeed;
            PreviousGameID = previousGameID;
            Score = score;
            IsWinner = isWinner;
        }
    }
}