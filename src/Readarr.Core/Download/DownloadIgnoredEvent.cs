using System.Collections.Generic;
using Readarr.Common.Messaging;
using Readarr.Core.Download.TrackedDownloads;
using Readarr.Core.Languages;
using Readarr.Core.Qualities;
using Readarr.Core.Tv;

namespace Readarr.Core.Download
{
    public class DownloadIgnoredEvent : IEvent
    {
        public int SeriesId { get; set; }
        public List<int> EpisodeIds { get; set; }
        public List<Language> Languages { get; set; }
        public QualityModel Quality { get; set; }
        public string SourceTitle { get; set; }
        public DownloadClientItemClientInfo DownloadClientInfo { get; set; }
        public string DownloadId { get; set; }
        public TrackedDownload TrackedDownload { get; set; }
        public string Message { get; set; }
    }
}
