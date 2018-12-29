using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Climb.Services
{
    public class FileStorageCdn : CdnService
    {
        private readonly string localCdnPath;

        public FileStorageCdn()
        {
            localCdnPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", @"temp\cdn");
            root = @"\temp\cdn";
        }

        protected override async Task UploadImageInternalAsync(IFormFile image, string folder, string fileKey)
        {
            var folderPath = GetFolderPath(folder);
            var filePath = Path.Combine(folderPath, fileKey);

            using(var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
            }
        }

        protected override void EnsureFolder(string folder)
        {
            var folderPath = GetFolderPath(folder);
            if(!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
        }

        public override Task DeleteImageAsync(string fileKey, ImageRules rules)
        {
            var folderPath = GetFolderPath(rules.Folder);
            var filePath = Path.Combine(folderPath, fileKey);

            if (File.Exists(filePath))
            {
                File.Delete(filePath); 
            }
            return Task.CompletedTask;
        }

        private string GetFolderPath(string folder) => Path.Combine(localCdnPath, folder);
    }
}