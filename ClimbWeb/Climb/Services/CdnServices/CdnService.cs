using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Climb.Services
{
    public abstract class CdnService : ICdnService
    {
        protected string root;

        protected abstract Task UploadImageInternalAsync(IFormFile image, string folder, string fileKey);
        protected abstract void EnsureFolder(string rulesFolder);
        public abstract Task DeleteImageAsync(string fileKey, ImageRules rules);

        public string GetImageUrl(string imageKey, ImageRules rules) => string.IsNullOrWhiteSpace(imageKey) ? rules.MissingUrl : $"{root}/{rules.Folder}/{imageKey}";

        public async Task<string> UploadImageAsync(IFormFile image, ImageRules rules)
        {
            if(!IsValid(image, rules))
            {
                throw new ArgumentException($"Image size {image.Length:N0}B exceeds limit {rules.MaxSize:N0}B.");
            }

            EnsureFolder(rules.Folder);

            var fileKey = GenerateFileKey(image);
            await UploadImageInternalAsync(image, rules.Folder, fileKey);
            return fileKey;
        }

        public async Task<string> ReplaceImageAsync(string oldKey, IFormFile image, ImageRules rules)
        {
            if(!string.IsNullOrWhiteSpace(oldKey))
            {
                await DeleteImageAsync(oldKey, rules);
            }

            return await UploadImageAsync(image, rules);
        }

        private static bool IsValid(IFormFile image, ImageRules rules)
        {
            return image.Length <= rules.MaxSize;
        }

        private static string GenerateFileKey(IFormFile file)
        {
            var fileExtension = Path.GetExtension(file.FileName);
            var fileName = Path.GetInvalidFileNameChars().Aggregate(Path.GetFileNameWithoutExtension(file.FileName), (current, c) => current.Replace(c, '_'));
            fileName = fileName.Replace(".", "");
            var fileKey = $"{fileName}_{Guid.NewGuid()}{fileExtension}";
            return fileKey;
        }
    }
}