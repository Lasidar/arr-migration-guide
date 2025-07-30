using System.Linq;
using Readarr.Core.Download;
using Readarr.Core.Parser;
using Readarr.Core.Parser.Model;

namespace Readarr.Core.MediaFiles.BookImport.Aggregation.Aggregators.Augmenters.Language
{
    public class AugmentLanguageFromDownloadClientItem : IAugmentLanguage
    {
        public int Order => 3;
        public string Name => "DownloadClientItem";

        public AugmentLanguageResult AugmentLanguage(LocalEpisode localEpisode, DownloadClientItem downloadClientItem)
        {
            var languages = localEpisode.DownloadClientEpisodeInfo?.Languages;

            if (languages == null)
            {
                return null;
            }

            foreach (var episode in localEpisode.Episodes)
            {
                var episodeTitleLanguage = LanguageParser.ParseLanguages(episode.Title);

                languages = languages.Except(episodeTitleLanguage).ToList();
            }

            return new AugmentLanguageResult(languages, Confidence.DownloadClientItem);
        }
    }
}
