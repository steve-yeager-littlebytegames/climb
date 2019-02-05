using System.Threading.Tasks;
using Climb.Models;

namespace Climb.Services
{
    public interface ITournamentService
    {
        Task<Tournament> Create(int leagueID, int? seasonID, string name);
        Task<Tournament> GenerateBracket(int tournamentID);
        void AddBracket(Tournament tournament, BracketGenerator.BracketData bracketData);
        Task<TournamentUser> Join(int tournamentID, string userID);
        Task Leave(int competitorID);
    }
}