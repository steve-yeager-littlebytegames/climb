namespace Climb
{
    public class ImageRules
    {
        public long MaxSize { get; }
        public string Folder { get; }
        public string MissingUrl { get; }

        public ImageRules(long maxSize, string folder, string missingUrl)
        {
            MaxSize = maxSize;
            Folder = folder;
            MissingUrl = missingUrl;
        }
    }
}