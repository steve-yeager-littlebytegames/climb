using System.Collections.Generic;
using System.Threading.Tasks;
using Climb.Models;

namespace Climb.Services.ModelServices
{
    public interface ILeagueService
    {
        Task<League> Create(string name, int gameID, string adminID);
        Task<LeagueUser> Join(int leagueID, string userID);
        Task<League> UpdateStandings(int leagueID);
        Task<IReadOnlyList<RankSnapshot>> TakeSnapshots(int leagueID);
        Task<List<Character>> GetUsersRecentCharactersAsync(int leagueUserID, int characterCount);
    }
}