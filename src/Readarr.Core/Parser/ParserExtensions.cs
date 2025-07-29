using System.Text.RegularExpressions;

namespace Readarr.Core.Parser
{
    public static class ParserExtensions
    {
        private static readonly Regex InvalidCharRegex = new Regex(@"[^a-zA-Z0-9\s\-]", RegexOptions.Compiled);
        private static readonly Regex MultipleSpacesRegex = new Regex(@"\s+", RegexOptions.Compiled);

        public static string CleanAuthorName(this string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return string.Empty;
            }

            // Remove invalid characters
            var cleaned = InvalidCharRegex.Replace(name, string.Empty);
            
            // Replace multiple spaces with single space
            cleaned = MultipleSpacesRegex.Replace(cleaned, " ");
            
            return cleaned.Trim().ToLowerInvariant();
        }

        public static string CleanBookTitle(this string title)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                return string.Empty;
            }

            // Remove invalid characters
            var cleaned = InvalidCharRegex.Replace(title, string.Empty);
            
            // Replace multiple spaces with single space
            cleaned = MultipleSpacesRegex.Replace(cleaned, " ");
            
            return cleaned.Trim().ToLowerInvariant();
        }

        public static string CleanSeriesTitle(this string title)
        {
            return CleanBookTitle(title);
        }
    }
}