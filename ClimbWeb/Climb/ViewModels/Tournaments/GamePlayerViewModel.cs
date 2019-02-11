using Climb.Models;

namespace Climb.ViewModels.Tournaments
{
    public class GamePlayerViewModel
    {
        public TournamentUser User { get; set; }
        public int PlayerSeed { get; }
        public int? PreviousGameID { get; }
        public int Score { get; }
        public bool IsWinner { get; }

        public bool HasUser => User != null;

        public GamePlayerViewModel(TournamentUser user, int playerSeed, int? previousGameID, int score, bool isWinner)
        {
            User = user;
            PlayerSeed = playerSeed;
            PreviousGameID = previousGameID;
            Score = score;
            IsWinner = isWinner;
        }
    }
}