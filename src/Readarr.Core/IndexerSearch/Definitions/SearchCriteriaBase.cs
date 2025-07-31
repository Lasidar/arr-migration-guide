using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Readarr.Common.EnsureThat;
using Readarr.Common.Extensions;
using Readarr.Core.DataAugmentation.Scene;
using Readarr.Core.Books;

namespace Readarr.Core.IndexerSearch.Definitions
{
    public abstract class SearchCriteriaBase
    {
        private static readonly Regex SpecialCharacter = new Regex(@"['.\u0060\u00B4\u2018\u2019]", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex NonWord = new Regex(@"[\W]", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex BeginningThe = new Regex(@"^the\s", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public Series Series { get; set; }
        public List<string> SceneTitles { get; set; }
        public List<Episode> Episodes { get; set; }
        public SearchMode SearchMode { get; set; }
        public virtual bool MonitoredBooksOnly { get; set; }
        public virtual bool UserInvokedSearch { get; set; }
        public virtual bool InteractiveSearch { get; set; }

        public List<int> IndexerIds { get; set; }
        public List<int> Categories { get; set; }

        public SearchCriteriaBase()
        {
            IndexerIds = new List<int>();
            Categories = new List<int>();
        }

        public bool IsRssSearch => !UserInvokedSearch && !InteractiveSearch;

        public override string ToString()
        {
            var args = new List<string>();

            if (MonitoredBooksOnly)
            {
                args.Add("MonitoredOnly");
            }

            if (UserInvokedSearch)
            {
                args.Add("UserInvoked");
            }

            if (InteractiveSearch)
            {
                args.Add("Interactive");
            }

            if (IndexerIds.Any())
            {
                args.Add($"IndexerIds: {string.Join(",", IndexerIds)}");
            }

            if (Categories.Any())
            {
                args.Add($"Categories: {string.Join(",", Categories)}");
            }

            return string.Join(", ", args);
        }

        public List<string> AllSceneTitles => SceneTitles.Concat(CleanSceneTitles).Distinct().ToList();
        public List<string> CleanSceneTitles => SceneTitles.Select(GetCleanSceneTitle).Distinct().ToList();

        public static string GetCleanSceneTitle(string title)
        {
            Ensure.That(title, () => title).IsNotNullOrWhiteSpace();

            var cleanTitle = BeginningThe.Replace(title, string.Empty);

            cleanTitle = cleanTitle.Replace("&", "and");
            cleanTitle = SpecialCharacter.Replace(cleanTitle, "");
            cleanTitle = NonWord.Replace(cleanTitle, "+");

            // remove any repeating +s
            cleanTitle = Regex.Replace(cleanTitle, @"\+{2,}", "+");
            cleanTitle = cleanTitle.RemoveAccent();
            return cleanTitle.Trim('+', ' ');
        }
    }
}
