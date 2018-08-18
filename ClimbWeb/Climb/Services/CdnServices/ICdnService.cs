using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Climb.Services
{
    public interface ICdnService
    {
        string GetImageUrl(string imageKey, ImageRules rules);
        Task DeleteImageAsync(string fileKey, ImageRules rules);
        Task<string> UploadImageAsync(IFormFile image, ImageRules rules);
        Task<string> ReplaceImageAsync(string oldKey, IFormFile image, ImageRules rules);
    }
}