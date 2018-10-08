using System.Collections.Generic;
using System.Threading.Tasks;
using Climb.Models;
using Climb.Requests.Sets;

namespace Climb.Services.ModelServices
{
    public interface ISetService
    {
        Task<Set> Update(int setID, IReadOnlyList<MatchForm> matchForms);
        Task<SetRequest> RequestSetAsync(int requesterID, int challengedID, string message);
        Task<SetRequest> RespondToSetRequestAsync(int requestID, bool accepted);
    }
}