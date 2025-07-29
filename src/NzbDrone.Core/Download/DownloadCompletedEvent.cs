using System.Collections.Generic;
using NzbDrone.Common.Messaging;
using NzbDrone.Core.Download.TrackedDownloads;
using NzbDrone.Core.MediaFiles;
using NzbDrone.Core.Parser.Model;

namespace NzbDrone.Core.Download
{
    public class DownloadCompletedEvent : IEvent
    {
        public TrackedDownload TrackedDownload { get; private set; }
        public int AuthorId { get; private set; }
        public List<EditionFile> EditionFiles { get; private set; }
        public GrabbedReleaseInfo Release { get; private set; }

        public DownloadCompletedEvent(TrackedDownload trackedDownload, int seriesId, List<EditionFile> episodeFiles, GrabbedReleaseInfo release)
        {
            TrackedDownload = trackedDownload;
            AuthorId = seriesId;
            EditionFiles = episodeFiles;
            Release = release;
        }
    }
}
