using System.Text.RegularExpressions;

namespace Readarr.Core.Parser
{
    public static class ParserExtensions
    {
        private static readonly Regex InvalidCharRegex = new Regex(@"[^a-zA-Z0-9\s\-]", RegexOptions.Compiled);
        private static readonly Regex MultipleSpacesRegex = new Regex(@"\s+", RegexOptions.Compiled);

        public static string CleanAuthorName(this string name)
        {
            // TODO: Implement author-specific name cleaning
            return CleanSeriesTitle(name);
        }
        
        public static string CleanBookTitle(this string title)
        {
            // TODO: Implement book-specific title cleaning
            return CleanSeriesTitle(title);
        }

        public static string CleanSeriesTitle(this string title)
        {
            return CleanBookTitle(title);
        }
    }
}