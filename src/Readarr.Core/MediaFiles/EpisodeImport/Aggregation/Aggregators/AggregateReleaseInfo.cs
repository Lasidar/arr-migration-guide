using System.Linq;
using Readarr.Common.Extensions;
using Readarr.Core.Download;
using Readarr.Core.History;
using Readarr.Core.Parser.Model;

namespace Readarr.Core.MediaFiles.EpisodeImport.Aggregation.Aggregators
{
    public class AggregateReleaseInfo : IAggregateLocalEpisode
    {
        public int Order => 1;

        private readonly IHistoryService _historyService;

        public AggregateReleaseInfo(IHistoryService historyService)
        {
            _historyService = historyService;
        }

        public LocalEpisode Aggregate(LocalEpisode localEpisode, DownloadClientItem downloadClientItem)
        {
            if (downloadClientItem == null)
            {
                return localEpisode;
            }

            var grabbedHistories = _historyService.FindByDownloadId(downloadClientItem.DownloadId)
                .Where(h => h.EventType == EpisodeHistoryEventType.Grabbed)
                .ToList();

            if (grabbedHistories.Empty())
            {
                return localEpisode;
            }

            localEpisode.Release = new GrabbedReleaseInfo(grabbedHistories);

            return localEpisode;
        }
    }
}
