using System.Threading.Tasks;
using Climb.Models;

namespace Climb.Services.ModelServices
{
    public interface IOrganizationService
    {
        Task<Organization> AddLeague(int organizationID, int leagueID, string userID);
    }
}