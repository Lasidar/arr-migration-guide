using System.Linq;
using Readarr.Core.Download;
using Readarr.Core.Parser;
using Readarr.Core.Parser.Model;

namespace Readarr.Core.MediaFiles.EpisodeImport.Aggregation.Aggregators.Augmenters.Language
{
    public class AugmentLanguageFromFolder : IAugmentLanguage
    {
        public int Order => 2;
        public string Name => "FolderName";

        public AugmentLanguageResult AugmentLanguage(LocalEpisode localEpisode, DownloadClientItem downloadClientItem)
        {
            var languages = localEpisode.FolderEpisodeInfo?.Languages;

            if (languages == null)
            {
                return null;
            }

            foreach (var episode in localEpisode.Episodes)
            {
                var episodeTitleLanguage = LanguageParser.ParseLanguages(episode.Title);

                languages = languages.Except(episodeTitleLanguage).ToList();
            }

            return new AugmentLanguageResult(languages, Confidence.Foldername);
        }
    }
}
