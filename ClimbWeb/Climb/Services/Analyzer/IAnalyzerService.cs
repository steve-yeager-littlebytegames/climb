using System.Collections.Generic;
using System.Threading.Tasks;

namespace Climb.Services
{
    public interface IAnalyzerService
    {
        Task<IReadOnlyList<string>> Calculate(int player1ID, int player2ID);
    }
}