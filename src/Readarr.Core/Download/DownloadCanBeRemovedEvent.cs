using Readarr.Common.Messaging;
using Readarr.Core.Download.TrackedDownloads;

namespace Readarr.Core.Download
{
    public class DownloadCanBeRemovedEvent : IEvent
    {
        public TrackedDownload TrackedDownload { get; private set; }

        public DownloadCanBeRemovedEvent(TrackedDownload trackedDownload)
        {
            TrackedDownload = trackedDownload;
        }
    }
}
