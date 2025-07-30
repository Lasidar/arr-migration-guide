using Readarr.Core.Download;
using Readarr.Core.Download.History;
using Readarr.Core.Parser;
using Readarr.Core.Parser.Model;
using Readarr.Core.Qualities;

namespace Readarr.Core.MediaFiles.BookImport.Aggregation.Aggregators.Augmenters.Quality
{
    public class AugmentQualityFromReleaseName : IAugmentQuality
    {
        public int Order => 5;
        public string Name => "ReleaseName";

        private readonly IDownloadHistoryService _downloadHistoryService;

        public AugmentQualityFromReleaseName(IDownloadHistoryService downloadHistoryService)
        {
            _downloadHistoryService = downloadHistoryService;
        }

        public AugmentQualityResult AugmentQuality(LocalEpisode localEpisode, DownloadClientItem downloadClientItem)
        {
            // Don't try to augment if we can't lookup the grabbed history by downloadId
            if (downloadClientItem == null)
            {
                return null;
            }

            var history = _downloadHistoryService.GetLatestGrab(downloadClientItem.DownloadId);

            if (history == null)
            {
                return null;
            }

            var historyQuality = QualityParser.ParseQuality(history.SourceTitle);

            var sourceConfidence = historyQuality.SourceDetectionSource == QualityDetectionSource.Name
                ? Confidence.Tag
                : Confidence.Fallback;

            var resolutionConfidence = historyQuality.ResolutionDetectionSource == QualityDetectionSource.Name
                ? Confidence.Tag
                : Confidence.Fallback;

            var revisionConfidence = historyQuality.RevisionDetectionSource == QualityDetectionSource.Name
                ? Confidence.Tag
                : Confidence.Fallback;

            return new AugmentQualityResult(historyQuality.Quality.Source, sourceConfidence, historyQuality.Quality.Resolution, resolutionConfidence, historyQuality.Revision, revisionConfidence);
        }
    }
}
