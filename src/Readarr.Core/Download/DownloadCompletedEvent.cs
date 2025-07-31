using System.Collections.Generic;
using Readarr.Common.Messaging;
using Readarr.Core.Download.TrackedDownloads;
using Readarr.Core.MediaFiles;
using Readarr.Core.Parser.Model;
using Readarr.Core.Tv;

namespace Readarr.Core.Download
{
    public class DownloadCompletedEvent : IEvent
    {
        public TrackedDownload TrackedDownload { get; private set; }
        public int SeriesId { get; private set; }
        public List<EpisodeFile> EpisodeFiles { get; private set; }
        public GrabbedReleaseInfo Release { get; private set; }

        public DownloadCompletedEvent(TrackedDownload trackedDownload, int seriesId, List<EpisodeFile> episodeFiles, GrabbedReleaseInfo release)
        {
            TrackedDownload = trackedDownload;
            SeriesId = seriesId;
            EpisodeFiles = episodeFiles;
            Release = release;
        }
    }
}
