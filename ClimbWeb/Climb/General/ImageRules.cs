namespace Climb
{
    public class ImageRules
    {
        public long MaxSize { get; }
        public int Width { get; }
        public int Height { get; }
        public string Folder { get; }
        public string MissingUrl { get; }

        public ImageRules(long maxSize, int width, int height, string folder, string missingUrl)
        {
            MaxSize = maxSize;
            Width = width;
            Height = height;
            Folder = folder;
            MissingUrl = missingUrl;
        }
    }
}