using System.Collections.Generic;
using System.Threading.Tasks;
using Climb.Models;
using Climb.Requests.Games;
using Microsoft.AspNetCore.Http;

namespace Climb.Services.ModelServices
{
    public interface IGameService
    {
        Task<Game> Update(UpdateRequest request);
        Task<Character> AddCharacter(int gameID, int? characterID, string name, IFormFile imageFile);
        Task<IReadOnlyCollection<Character>> AddCharacters(int gameID, ICollection<string> names);
        Task<Stage> AddStage(int gameID, int? stageID, string name);
    }
}