using System.Threading.Tasks;

namespace Climb.Services
{
    public interface IAnalyzerService
    {
        Task<AnalyzerDataCollection> Calculate(int player1ID, int player2ID);
    }
}