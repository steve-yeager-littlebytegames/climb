namespace Climb.Extensions
{
    public static class LongExtensions
    {
        // https://stackoverflow.com/a/11124118
        // Returns the human-readable file size for an arbitrary, 64-bit file size 
        // The default format is "0.### XB", e.g. "4.2 KB" or "1.434 GB"
        public static string ToBytesReadable(this long value)
        {
            // Get absolute value
            var absoluteI = value < 0 ? -value : value;
            // Determine the suffix and readable value
            string suffix;
            double readable;
            if(absoluteI >= 0x1000000000000000) // Exabyte
            {
                suffix = "EB";
                readable = value >> 50;
            }
            else if(absoluteI >= 0x4000000000000) // Petabyte
            {
                suffix = "PB";
                readable = value >> 40;
            }
            else if(absoluteI >= 0x10000000000) // Terabyte
            {
                suffix = "TB";
                readable = value >> 30;
            }
            else if(absoluteI >= 0x40000000) // Gigabyte
            {
                suffix = "GB";
                readable = value >> 20;
            }
            else if(absoluteI >= 0x100000) // Megabyte
            {
                suffix = "MB";
                readable = value >> 10;
            }
            else if(absoluteI >= 0x400) // Kilobyte
            {
                suffix = "KB";
                readable = value;
            }
            else
            {
                return value.ToString("0 B"); // Byte
            }

            // Divide by 1024 to get fractional value
            readable = readable / 1024;
            // Return formatted number with suffix
            return readable.ToString("0.### ") + suffix;
        }
    }
}