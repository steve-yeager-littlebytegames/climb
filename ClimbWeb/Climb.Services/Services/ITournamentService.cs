using System.Threading.Tasks;
using Climb.Models;

namespace Climb.Services
{
    public interface ITournamentService
    {
        Task<Tournament> Create(int leagueID, int? seasonID, string name);
        Task<TournamentUser> Join(int tournamentID, string userID);
        Task Leave(int competitorID);
        Task<Tournament> Start(int tournamentID);

        // Private?
        //void CreateBracket(Tournament tournament);
        //void PopulateBracket(Tournament tournament);
    }
}