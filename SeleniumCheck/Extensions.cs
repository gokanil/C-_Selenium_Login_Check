using System.Text;

namespace SeleniumCheck
{
    internal static class Extensions
    {
        public static string ToBase64(this string text)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(text));
        }

        public static string BytesToMegabytes(this long bytes)
        {
            return String.Format("{0:0.##}", (bytes / 1024f) / 1024f);
        }
    }
}
