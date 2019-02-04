using System.Threading.Tasks;
using Climb.Models;

namespace Climb.Services
{
    public interface ITournamentService
    {
        Task<Tournament> Create(int leagueID, int? seasonID, string name);
        Task<Tournament> GenerateBracket(int tournamentID);
        Task<TournamentUser> Join(int tournamentID, string userID);
        Task Leave(int competitorID);
    }
}